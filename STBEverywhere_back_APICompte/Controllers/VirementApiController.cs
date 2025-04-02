using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.DTO;
using Microsoft.AspNetCore.Http;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APICompte.Repository.IRepository;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using STBEverywhere_back_APICompte.Services;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using STBEverywhere_ApiAuth.Repositories;
using STBEverywhere_back_APIClient.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
//using STBEverywhere_back_APIClient.Repositories;


namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/virement")]
    [ApiController]
    public class VirementApiController : ControllerBase
    {

        private readonly ICompteRepository _dbCompte;
        private readonly IVirementRepository _dbVirement;
        private readonly IFraisCompteRepository _dbFraisCompte;
        private readonly IBeneficiaireRepository _dbBeneficiaire;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<VirementApiController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IVirementService _virementService;


        public VirementApiController(/*VirementService virementService,*/ IWebHostEnvironment webHostEnvironment, IFraisCompteRepository dbFraisCompte, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository,ICompteRepository dbCompte, IVirementRepository dbVirement, IBeneficiaireRepository dbBeneficiaire, ILogger<VirementApiController> logger, IMapper mapper)
        {
            _dbCompte = dbCompte;
            _dbFraisCompte = dbFraisCompte;
            _dbBeneficiaire = dbBeneficiaire;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            // _virementService = virementService;

        }



        [HttpPost("Virement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public async Task<IActionResult> Virement([FromBody] VirementUnitaireDto virementDto)
        {
            _logger.LogInformation("Requête reçue pour un virement. Données : {@virementDto}", virementDto);

            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client?.Id;

            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            if (virementDto == null || string.IsNullOrEmpty(virementDto.RIB_Emetteur) || virementDto.Montant <= 0)
            {
                return BadRequest(new { message = "RIB émetteur et montant sont obligatoires." });
            }

            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Emetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                return NotFound(new { message = "Compte émetteur introuvable." });
            }

            Compte recepteur;
            if (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef")
            {
                if (virementDto.IdBeneficiaire == null)
                {
                    return BadRequest(new { message = "ID bénéficiaire requis pour ce type de virement." });
                }

                var beneficiaire = await _dbBeneficiaire.GetByIdAsync(virementDto.IdBeneficiaire.Value);
                if (beneficiaire == null)
                {
                    return NotFound(new { message = "Bénéficiaire introuvable." });
                }

                recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == beneficiaire.RIBCompte)).FirstOrDefault();
            }
            else if (virementDto.TypeVirement == "VirementUnitaireVersMescomptes")
            {
                if (string.IsNullOrEmpty(virementDto.RIB_Recepteur))
                {
                    return BadRequest(new { message = "RIB récepteur requis pour ce type de virement." });
                }

                recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Recepteur)).FirstOrDefault();
            }
            else
            {
                return BadRequest(new { message = "Type de virement non reconnu." });
            }

            if (recepteur == null)
            {
                return NotFound(new { message = "Compte récepteur introuvable." });
            }

            decimal frais = 0.5m;
            decimal montantTotal = virementDto.Montant + (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef" ? frais : 0);

            // ⚠️ Vérification du solde disponible (incluant le découvert)
            if (emetteur.SoldeDisponible < montantTotal)
            {
                return BadRequest(new { message = "Solde insuffisant, y compris avec le découvert autorisé." });
            }

            await _dbVirement.BeginTransactionAsync();
            try
            {
                // 📉 Débit du compte émetteur
                emetteur.Solde -= montantTotal;

                // ⚠️ Mise à jour du découvert autorisé si nécessaire
                if (emetteur.Solde < 0)
                {
                    emetteur.DecouvertAutorise += emetteur.Solde; // Réduit le découvert autorisé de la partie négative
                }

                // 📈 Crédit du compte récepteur
                recepteur.Solde += virementDto.Montant;

                await _dbCompte.UpdateAsync(emetteur);
                await _dbCompte.UpdateAsync(recepteur);

                var virement = new Virement
                {
                    RIB_Emetteur = virementDto.RIB_Emetteur,
                    RIB_Recepteur = recepteur.RIB,
                    Montant = virementDto.Montant,
                    DateVirement = DateTime.Now,
                    Statut = "Réussi",
                    Motif = virementDto.motif,
                    TypeVirement = virementDto.TypeVirement,
                    Description = virementDto.Description
                };

                await _dbVirement.CreateAsync(virement);

                if (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef")
                {
                    var fraisEmetteur = new FraisCompte
                    {
                        RIB = virementDto.RIB_Emetteur,
                        IdsVirements = new List<int> { virement.Id },
                        Montant = frais,
                        type = "Virement émis",
                        Date = DateTime.Now
                    };

                    var fraisRecepteur = new FraisCompte
                    {
                        RIB = recepteur.RIB,
                        IdsVirements = new List<int> { virement.Id },
                        Montant = frais,
                        type = "Virement reçu",
                        Date = DateTime.Now
                    };

                    await _dbFraisCompte.CreateAsync(fraisEmetteur);
                    await _dbFraisCompte.CreateAsync(fraisRecepteur);
                }

                await _dbVirement.CommitTransactionAsync();

                return Ok(new { message = "Virement effectué avec succès." });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du virement");
                return StatusCode(500, new { message = "Erreur lors du traitement du virement." });
            }
        }


        /* public async Task<IActionResult> Virement([FromBody] VirementUnitaireDto virementDto)
         {



                 _logger.LogInformation("Requête reçue pour un virement. Données : {@virementDto}", virementDto);
                 var userId = GetUserIdFromToken();
                 var client = await _userRepository.GetClientByUserIdAsync(userId);
                 var clientId = client.Id;
                 if (clientId == null)
                 {
                     return Unauthorized(new { message = "Utilisateur non authentifié" });
                 }

                 if (virementDto == null || string.IsNullOrEmpty(virementDto.RIB_Emetteur) || virementDto.Montant <= 0)
                 {
                     return BadRequest(new { message = "RIB émetteur et montant sont obligatoires." });
                 }

                 var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Emetteur)).FirstOrDefault();
                 if (emetteur == null)
                 {
                     return NotFound(new { message = "Compte émetteur introuvable." });
                 }

                 Compte recepteur;
                 if (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef")
                 {
                     if (virementDto.IdBeneficiaire == null)
                     {
                         return BadRequest(new { message = "ID bénéficiaire requis pour ce type de virement." });
                     }

                     var beneficiaire = await _dbBeneficiaire.GetByIdAsync(virementDto.IdBeneficiaire.Value);
                     if (beneficiaire == null)
                     {
                         return NotFound(new { message = "Bénéficiaire introuvable." });
                     }

                     recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == beneficiaire.RIBCompte)).FirstOrDefault();
                 }
                 else if (virementDto.TypeVirement == "VirementUnitaireVersMescomptes")
                 {
                     if (string.IsNullOrEmpty(virementDto.RIB_Recepteur))
                     {
                         return BadRequest(new { message = "RIB récepteur requis pour ce type de virement." });
                     }

                     recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Recepteur)).FirstOrDefault();
                 }
                 else
                 {
                     return BadRequest(new { message = "Type de virement non reconnu." });
                 }

                 if (recepteur == null)
                 {
                     return NotFound(new { message = "Compte récepteur introuvable." });
                 }

                 decimal frais = 0.5m;
                 decimal montantTotal = virementDto.Montant + (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef" ? frais : 0);

                 if (emetteur.Solde < montantTotal)
                 {
                     return BadRequest(new { message = "Solde insuffisant." });
                 }

                 await _dbVirement.BeginTransactionAsync();
                 try
                 {
                     emetteur.Solde -= montantTotal;
                     recepteur.Solde -= (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef" ? frais : 0);
                     recepteur.Solde += virementDto.Montant;

                     await _dbCompte.UpdateAsync(emetteur);
                     await _dbCompte.UpdateAsync(recepteur);

                     var virement = new Virement
                     {
                         RIB_Emetteur = virementDto.RIB_Emetteur,
                         RIB_Recepteur = recepteur.RIB,
                         Montant = virementDto.Montant,
                         DateVirement = DateTime.Now,
                         Statut = "Réussi",
                         Motif = virementDto.motif,
                         TypeVirement = virementDto.TypeVirement,
                         Description = virementDto.Description
                     };

                     await _dbVirement.CreateAsync(virement);

                     if (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef")
                     {
                         var fraisEmetteur = new FraisCompte
                         {
                             RIB = virementDto.RIB_Emetteur,

                             IdsVirements = new List<int> { virement.Id },
                             Montant = frais,
                             type = "Virement émis",
                             Date = DateTime.Now
                         };

                         var fraisRecepteur = new FraisCompte
                         {
                             RIB = recepteur.RIB,
                             //IdVirement = virement.Id,
                             IdsVirements = new List<int> { virement.Id },
                             Montant = frais,
                             type = "Virement reçu",
                             Date = DateTime.Now
                         };

                         await _dbFraisCompte.CreateAsync(fraisEmetteur);
                         await _dbFraisCompte.CreateAsync(fraisRecepteur);
                     }

                     await _dbVirement.CommitTransactionAsync();

                     return Ok(new { message = "Virement effectué avec succès." });
                 }
                 catch (Exception ex)
                 {
                     await _dbVirement.RollbackTransactionAsync();
                     _logger.LogError(ex, "Erreur lors du virement");
                     return StatusCode(500, new { message = "Erreur lors du traitement du virement." });
                 }
             }*/





        /*_logger.LogInformation("Requête reçue pour un virement. Données : {@virementDto}", virementDto);
        var userId = GetUserIdFromToken();
        var client = await _userRepository.GetClientByUserIdAsync(userId);
        var clientId = client.Id;
        if (clientId == null)
        {
            return Unauthorized(new { message = "Utilisateur non authentifié" });
        }

        // Validation de base
        if (virementDto == null || string.IsNullOrEmpty(virementDto.RIB_Emetteur) || virementDto.Montant <= 0)
        {
            return BadRequest(new { message = "RIB émetteur et montant sont obligatoires." });
        }

        // Vérification du compte émetteur
        var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Emetteur)).FirstOrDefault();
        if (emetteur == null)
        {
            return NotFound(new { message = "Compte émetteur introuvable." });
        }

        // Gestion différente selon le type de virement
        Compte recepteur;
        if (virementDto.TypeVirement == "VirementUnitaireVersAutreBenef" )
        {
            if (virementDto.IdBeneficiaire == null)
            {
                return BadRequest(new { message = "ID bénéficiaire requis pour ce type de virement." });
            }

            var beneficiaire = await _dbBeneficiaire.GetByIdAsync(virementDto.IdBeneficiaire.Value);
            if (beneficiaire == null)
            {
                return NotFound(new { message = "Bénéficiaire introuvable." });
            }

            recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == beneficiaire.RIBCompte)).FirstOrDefault();
        }
        else if (virementDto.TypeVirement == "VirementUnitaireVersMescomptes"  )
        {
            if (string.IsNullOrEmpty(virementDto.RIB_Recepteur))
            {
                return BadRequest(new { message = "RIB récepteur requis pour ce type de virement." });
            }

            recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Recepteur)).FirstOrDefault();
        }
        else
        {
            return BadRequest(new { message = "Type de virement non reconnu." });
        }

        if (recepteur == null)
        {
            return NotFound(new { message = "Compte récepteur introuvable." });
        }

        // Vérification du solde
        if (emetteur.Solde < virementDto.Montant)
        {
            return BadRequest(new { message = "Solde insuffisant." });
        }

        // Exécution du virement
        await _dbVirement.BeginTransactionAsync();
        try
        {
            emetteur.Solde -= virementDto.Montant;
            recepteur.Solde += virementDto.Montant;

            await _dbCompte.UpdateAsync(emetteur);
            await _dbCompte.UpdateAsync(recepteur);

            var virement = new Virement
            {
                RIB_Emetteur = virementDto.RIB_Emetteur,
                RIB_Recepteur = recepteur.RIB,
                Montant = virementDto.Montant,
                DateVirement = DateTime.Now,
                Statut = "Réussi",
                Motif = virementDto.motif,
                TypeVirement = virementDto.TypeVirement,
                Description = virementDto.Description
            };

            await _dbVirement.CreateAsync(virement);
            await _dbVirement.CommitTransactionAsync();

            return Ok(new { message = "Virement effectué avec succès." });
        }
        catch (Exception ex)
        {
            await _dbVirement.RollbackTransactionAsync();
            _logger.LogError(ex, "Erreur lors du virement");
            return StatusCode(500, new { message = "Erreur lors du traitement du virement." });
        }
    }*/





        [HttpPost("VirementDeMasseForm")]
       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VirementDeMasseForm([FromBody] VirementMasseFormulaireDto dto)
        {
           
              
                if (dto == null || dto.Beneficiaires == null || !dto.Beneficiaires.Any())
                {
                    return BadRequest(new { message = "Données invalides : la liste des bénéficiaires est requise." });
                }

                // Appel du traitement spécifique au formulaire
                return await VirementDeMasseParFormulaire(dto);
            }

           
        



        [HttpPost("VirementDeMasseFile")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> VirementDeMasseFile([FromForm] UploadFichierRequest request)
        {

            if (request.Fichier == null || request.Fichier.Length == 0)
            {
                return BadRequest(new { message = "Fichier invalide ou vide." });
            }

            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, request.Fichier.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Fichier.CopyToAsync(stream);
            }

            _logger.LogInformation("Fichier uploadé avec succès : {FilePath}", filePath);

            // Appel direct du traitement de virement
            return await VirementDeMasseTraitement(filePath);
        }

        /*
                public async Task<IActionResult> UploadFichier([FromForm] UploadFichierRequest request)
                {
                    if (request.Fichier == null || request.Fichier.Length == 0)
                    {
                        return BadRequest(new { message = "Fichier invalide ou vide." });
                    }

                    var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var filePath = Path.Combine(uploadsPath, request.Fichier.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Fichier.CopyToAsync(stream);
                    }

                    return Ok(new { message = "Fichier uploadé avec succès.", filePath });
                }*/
        private async Task<IActionResult> VirementDeMasseParFormulaire(VirementMasseFormulaireDto dto)
        {
            var virementsEffectués = new List<Virement>();

            // Récupération du compte émetteur
            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == dto.RibEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                return BadRequest(new { message = "Compte émetteur introuvable." });
            }

            // Calcul du total des virements
            decimal totalVirement = dto.Beneficiaires.Sum(b => b.Montant);

            // Calcul des frais
            int nombreBeneficiaires = dto.Beneficiaires.Count;
            decimal frais = 5.0m; // Frais de base pour les 5 premiers bénéficiaires
            if (nombreBeneficiaires > 5)
            {
                frais += (nombreBeneficiaires - 5) * 0.5m;
            }

            // Calcul du solde disponible (en tenant compte du découvert)
            decimal soldeDisponible = emetteur.Solde + emetteur.DecouvertAutorise;

            // Vérification du solde disponible
            if (soldeDisponible < (totalVirement + frais))
            {
                return BadRequest(new { message = $"Solde insuffisant, y compris avec le découvert autorisé. Montant nécessaire : {totalVirement + frais}" });
            }

            await _dbVirement.BeginTransactionAsync();
            try
            {
                var idsVirements = new List<int>();

                foreach (var beneficiaire in dto.Beneficiaires)
                {
                    var ribRecepteur = await GetRibById(beneficiaire.IdBeneficiaire);
                    if (string.IsNullOrEmpty(ribRecepteur))
                    {
                        await _dbVirement.RollbackTransactionAsync();
                        return BadRequest(new { message = $"RIB introuvable pour le bénéficiaire ID {beneficiaire.IdBeneficiaire}." });
                    }

                    var recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribRecepteur)).FirstOrDefault();
                    if (recepteur == null)
                    {
                        await _dbVirement.RollbackTransactionAsync();
                        return BadRequest(new { message = $"Compte destinataire introuvable pour le bénéficiaire ID {beneficiaire.IdBeneficiaire}." });
                    }

                    // Débit/Crédit en tenant compte du découvert
                    emetteur.Solde -= beneficiaire.Montant;
                    if (idsVirements.Count == 0) // Premier virement, on déduit les frais
                    {
                        emetteur.Solde -= frais;
                    }
                    recepteur.Solde += beneficiaire.Montant;

                    // Ajustement du découvert autorisé si le solde devient négatif
                    if (emetteur.Solde < 0)
                    {
                        emetteur.DecouvertAutorise += emetteur.Solde; // Réduction du découvert autorisé
                    }

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(recepteur);

                    // Création du virement
                    var virement = new Virement
                    {
                        RIB_Emetteur = dto.RibEmetteur,
                        RIB_Recepteur = ribRecepteur,
                        Motif = dto.Motif,
                        Description = dto.Description,
                        DateVirement = DateTime.Now,
                        Montant = beneficiaire.Montant,
                        Statut = "Réussi",
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = null,
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                    idsVirements.Add(virement.Id);
                }

                // Enregistrement des frais
                var fraisCompte = new FraisCompte
                {
                    RIB = dto.RibEmetteur,
                    type = "Virement multiple",
                    Date = DateTime.Now,
                    Montant = frais,
                    IdsVirements = idsVirements // Utilisation de la propriété NotMapped
                };

                await _dbFraisCompte.CreateAsync(fraisCompte);

                await _dbVirement.CommitTransactionAsync();
                return Ok(new
                {
                    message = "Virements enregistrés avec succès.",
                    virementsEffectués,
                    frais = frais,
                    nombreVirements = idsVirements.Count
                });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                return StatusCode(500, new { message = "Erreur lors du traitement du virement.", erreur = ex.Message });
            }
        }

        private async Task<string?> GetRibById(int idBeneficiaire)
        {
            var beneficiaire = await _dbBeneficiaire.GetByIdAsync(idBeneficiaire);
            return beneficiaire?.RIBCompte;
        }


        /*
        private async Task<IActionResult> VirementDeMasseParFormulaire(VirementMasseFormulaireDto dto)
        {
            var virementsEffectués = new List<Virement>();

            // Récupération du compte émetteur
            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == dto.RibEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                return BadRequest(new { message = "Compte émetteur introuvable." });
            }

            // Calcul du total des virements
            decimal totalVirement = dto.Beneficiaires.Sum(b => b.Montant);

            // Calcul des frais
            int nombreBeneficiaires = dto.Beneficiaires.Count;
            decimal frais = 5.0m; // Frais de base pour les 5 premiers bénéficiaires
            if (nombreBeneficiaires > 5)
            {
                frais += (nombreBeneficiaires - 5) * 0.5m;
            }

            // Vérification du solde
            if (emetteur.Solde < (totalVirement + frais))
            {
                return BadRequest(new { message = $"Solde insuffisant. Montant nécessaire : {totalVirement + frais}" });
            }

            await _dbVirement.BeginTransactionAsync();
            try
            {
                var idsVirements = new List<int>();

                foreach (var beneficiaire in dto.Beneficiaires)
                {
                    var ribRecepteur = await GetRibById(beneficiaire.IdBeneficiaire);
                    if (string.IsNullOrEmpty(ribRecepteur))
                    {
                        await _dbVirement.RollbackTransactionAsync();
                        return BadRequest(new { message = $"RIB introuvable pour le bénéficiaire ID {beneficiaire.IdBeneficiaire}." });
                    }

                    var recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribRecepteur)).FirstOrDefault();
                    if (recepteur == null)
                    {
                        await _dbVirement.RollbackTransactionAsync();
                        return BadRequest(new { message = $"Compte destinataire introuvable pour le bénéficiaire ID {beneficiaire.IdBeneficiaire}." });
                    }

                    // Débit/Crédit
                    emetteur.Solde -= beneficiaire.Montant;
                    if (idsVirements.Count == 0) // Premier virement, on déduit les frais
                    {
                        emetteur.Solde -= frais;
                    }
                    recepteur.Solde += beneficiaire.Montant;

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(recepteur);

                    // Création du virement
                    var virement = new Virement
                    {
                        RIB_Emetteur = dto.RibEmetteur,
                        RIB_Recepteur = ribRecepteur,
                        Motif = dto.Motif,
                        Description = dto.Description,
                        DateVirement = DateTime.Now,
                        Montant = beneficiaire.Montant,
                        Statut = "Réussi",
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = null,
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                    idsVirements.Add(virement.Id);
                }

                // Enregistrement des frais
                var fraisCompte = new FraisCompte
                {
                    RIB = dto.RibEmetteur,
                    type = "Virement multiple",
                    Date = DateTime.Now,
                    Montant = frais,
                    IdsVirements = idsVirements // Utilisation de la propriété NotMapped
                };

                await _dbFraisCompte.CreateAsync(fraisCompte);

                await _dbVirement.CommitTransactionAsync();
                return Ok(new
                {
                    message = "Virements enregistrés avec succès.",
                    virementsEffectués,
                    frais = frais,
                    nombreVirements = idsVirements.Count
                });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                return StatusCode(500, new { message = "Erreur lors du traitement du virement.", erreur = ex.Message });
            }
        }

        private async Task<string?> GetRibById(int idBeneficiaire)
        {
            var beneficiaire = await _dbBeneficiaire.GetByIdAsync(idBeneficiaire);
            return beneficiaire?.RIBCompte;
        }*/


        /* private async Task<IActionResult> VirementDeMasseParFormulaire(VirementMasseFormulaireDto dto)
         {
             var virementsEffectués = new List<Virement>();

             // Récupération du compte émetteur
             var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == dto.RibEmetteur)).FirstOrDefault();
             if (emetteur == null)
             {
                 return BadRequest(new { message = "Compte émetteur introuvable." });
             }

             // Calcul du total des virements
             decimal totalVirement = dto.Beneficiaires.Sum(b => b.Montant);

             // Calcul des frais
             int nombreBeneficiaires = dto.Beneficiaires.Count;
             decimal frais = 5.0m; // Frais de base pour les 5 premiers bénéficiaires
             if (nombreBeneficiaires > 5)
             {
                 frais += (nombreBeneficiaires - 5) * 0.5m;
             }

             // Vérifier si l'émetteur a assez de solde (total virements + frais)
             if (emetteur.Solde < (totalVirement + frais))
             {
                 return BadRequest(new { message = $"Solde insuffisant pour effectuer tous les virements. Montant nécessaire : {totalVirement + frais}" });
             }

             await _dbVirement.BeginTransactionAsync();
             try
             {
                 int idPremierVirement = 0; // Pour stocker l'ID du premier virement
                 bool fraisDeduits = false; // Pour s'assurer qu'on déduit les frais une seule fois

                 foreach (var beneficiaire in dto.Beneficiaires)
                 {
                     var ribRecepteur = await GetRibById(beneficiaire.IdBeneficiaire);
                     if (string.IsNullOrEmpty(ribRecepteur))
                     {
                         await _dbVirement.RollbackTransactionAsync();
                         return BadRequest(new { message = $"Aucun RIB trouvé pour le bénéficiaire ID {beneficiaire.IdBeneficiaire}." });
                     }

                     var recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribRecepteur)).FirstOrDefault();
                     if (recepteur == null)
                     {
                         await _dbVirement.RollbackTransactionAsync();
                         return BadRequest(new { message = $"Compte destinataire introuvable pour le bénéficiaire ID {beneficiaire.IdBeneficiaire}." });
                     }

                     // Débiter l'émetteur (montant + frais si premier virement)
                     emetteur.Solde -= beneficiaire.Montant;
                     if (!fraisDeduits)
                     {
                         emetteur.Solde -= frais;
                         fraisDeduits = true;
                     }

                     // Créditer le récepteur
                     recepteur.Solde += beneficiaire.Montant;

                     await _dbCompte.UpdateAsync(emetteur);
                     await _dbCompte.UpdateAsync(recepteur);

                     // Création du virement
                     var virement = new Virement
                     {
                         RIB_Emetteur = dto.RibEmetteur,
                         RIB_Recepteur = ribRecepteur,
                         Motif = dto.Motif,
                         Description = dto.Description,
                         DateVirement = DateTime.Now,
                         Montant = beneficiaire.Montant,
                         Statut = "Réussi",
                         TypeVirement = "VirementDeMasse",
                         FichierBeneficaires = null,
                     };

                     await _dbVirement.CreateAsync(virement);
                     virementsEffectués.Add(virement);

                     // Garder l'ID du premier virement pour l'enregistrement des frais
                     if (idPremierVirement == 0)
                     {
                         idPremierVirement = virement.Id;
                     }
                 }

                 // Enregistrement des frais dans la table FraisCompte
                 var fraisCompte = new FraisCompte
                 {
                     RIB = dto.RibEmetteur,
                     type = "Virement multiple",
                     Date = DateTime.Now,
                     Montant = frais,
                     IdVirement = idPremierVirement
                 };
                 await _dbFraisCompte.CreateAsync(fraisCompte);

                 await _dbVirement.CommitTransactionAsync();
                 return Ok(new
                 {
                     message = "Virements enregistrés avec succès.",
                     virementsEffectués,
                     frais = frais
                 });
             }
             catch (Exception ex)
             {
                 await _dbVirement.RollbackTransactionAsync();
                 return StatusCode(500, new { message = "Erreur lors du traitement du virement.", erreur = ex.Message });
             }
         }

         private async Task<string?> GetRibById(int idBeneficiaire)
         {
             var beneficiaire = await _dbBeneficiaire.GetByIdAsync(idBeneficiaire);
             return beneficiaire?.RIBCompte;
         }*/





        private async Task<IActionResult> VirementDeMasseTraitement(string fichier)
        {
            if (string.IsNullOrEmpty(fichier) || !System.IO.File.Exists(fichier))
            {
                _logger.LogWarning("Fichier introuvable : {FichierPath}", fichier);
                return BadRequest(new { message = "Fichier introuvable." });
            }

            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;

            if (clientId == null)
            {
                _logger.LogWarning("Utilisateur non authentifié.");
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            _logger.LogInformation("Début du traitement du virement en masse pour le fichier : {FichierPath}", fichier);

            string[] lines;
            try
            {
                lines = await System.IO.File.ReadAllLinesAsync(fichier);
                if (lines.Length < 4 || !lines[0].StartsWith("DEBUT FICHIER STB"))
                {
                    _logger.LogWarning("Format de fichier invalide.");
                    return BadRequest(new { message = "Format de fichier invalide." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du fichier.");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la lecture du fichier." });
            }

            var ribEmetteur = lines[1].Substring(1, 20).Trim();
            var montantTotalString = Regex.Match(lines[1], @"STB\s*(\d+)").Groups[1].Value;
            if (!decimal.TryParse(montantTotalString, out var montantTotal))
            {
                _logger.LogWarning("Le montant total n'est pas dans un format valide : {Montant}", montantTotalString);
                return BadRequest(new { message = "Le montant total n'est pas dans un format valide." });
            }

            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                _logger.LogWarning("Compte émetteur introuvable : {RIB}", ribEmetteur);
                return NotFound(new { message = "Compte émetteur introuvable." });
            }

            int nombreBeneficiaires = lines.Length - 3;
            decimal frais = 5.0m + Math.Max(0, (nombreBeneficiaires - 5) * 0.5m);
            decimal soldeDisponible = emetteur.Solde + emetteur.DecouvertAutorise;

            if (soldeDisponible < (montantTotal + frais))
            {
                _logger.LogWarning("Solde insuffisant pour l'émetteur {RIB}. Solde disponible : {SoldeDisponible}, Montant requis : {MontantTotal}, Frais : {Frais}",
                    ribEmetteur, soldeDisponible, montantTotal, frais);
                return BadRequest(new { message = $"Solde insuffisant, y compris avec le découvert autorisé. Montant nécessaire : {montantTotal + frais}" });
            }

            await _dbVirement.BeginTransactionAsync();
            _logger.LogInformation("Transaction de virement en masse commencée.");

            try
            {
                var idsVirements = new List<int>();
                decimal totalDébité = 0;
                var virementsEffectués = new List<Virement>();

                for (int i = 2; i < lines.Length - 1; i++)
                {
                    var beneficiaireLigne = lines[i].Trim();
                    var matchBenef = Regex.Match(beneficiaireLigne, @"^B(\d{20})(.+?)\s+(\d+)(\w+)$");

                    if (!matchBenef.Success)
                    {
                        _logger.LogWarning("Format du bénéficiaire invalide. Ligne reçue: {Ligne}", beneficiaireLigne);
                        return BadRequest(new { message = "Format du bénéficiaire invalide." });
                    }

                    var ribBeneficiaire = matchBenef.Groups[1].Value.Trim();
                    var nomBeneficiaire = matchBenef.Groups[2].Value.Trim();
                    var montantStr = matchBenef.Groups[3].Value.Trim();
                    var motif = matchBenef.Groups[4].Value.Trim();

                    if (!decimal.TryParse(montantStr, out var montant))
                    {
                        _logger.LogWarning("Montant invalide pour le bénéficiaire {RIB} : {Montant}", ribBeneficiaire, montantStr);
                        return BadRequest(new { message = "Montant invalide dans le fichier." });
                    }

                    var beneficiaire = (await _dbCompte.GetAllAsync(c => c.RIB == ribBeneficiaire)).FirstOrDefault();
                    if (beneficiaire == null)
                    {
                        _logger.LogWarning("Compte bénéficiaire introuvable : {RIB}", ribBeneficiaire);
                        return NotFound(new { message = "Compte bénéficiaire introuvable." });
                    }

                    // Vérifier si le bénéficiaire est déjà enregistré pour ce client
                    var beneficiaireExistant = await _dbBeneficiaire.GetAllAsync(b =>
                        b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                    int beneficiaireId;
                    if (!beneficiaireExistant.Any())
                    {
                        var nouveauBeneficiaire = new Beneficiaire
                        {
                            ClientId = clientId,
                            Nom = nomBeneficiaire,
                            RIBCompte = ribBeneficiaire,
                            Prenom = nomBeneficiaire,
                        };

                        await _dbBeneficiaire.CreateAsync(nouveauBeneficiaire);
                        _logger.LogInformation("Nouveau bénéficiaire enregistré: {Nom}, RIB: {RIB}", nomBeneficiaire, ribBeneficiaire);

                        var beneficiaireCree = await _dbBeneficiaire.GetAllAsync(b =>
                            b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                        beneficiaireId = beneficiaireCree.First().Id;
                    }
                    else
                    {
                        beneficiaireId = beneficiaireExistant.First().Id;
                    }

                    // Débit/Crédit en tenant compte du découvert
                    emetteur.Solde -= montant;
                    if (i == 2) // Premier virement, on déduit les frais
                    {
                        emetteur.Solde -= frais;
                    }
                    beneficiaire.Solde += montant;

                    if (emetteur.Solde < 0)
                    {
                        emetteur.DecouvertAutorise += emetteur.Solde;
                    }

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(beneficiaire);

                    var virement = new Virement
                    {
                        RIB_Emetteur = ribEmetteur,
                        RIB_Recepteur = ribBeneficiaire,
                        Montant = montant,
                        DateVirement = DateTime.Now,
                        Statut = "Réussi",
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = fichier,
                        Description = $"Virement de masse depuis {ribEmetteur}",
                        Motif = motif
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                    idsVirements.Add(virement.Id);
                    totalDébité += montant;
                }

                var fraisCompte = new FraisCompte
                {
                    RIB = ribEmetteur,
                    type = "Virement multiple",
                    Date = DateTime.Now,
                    Montant = frais,
                    IdsVirements = idsVirements
                };
                await _dbFraisCompte.CreateAsync(fraisCompte);

                if (totalDébité != montantTotal)
                {
                    return BadRequest(new { message = $"Total débité {totalDébité} différent du Montant à débiter Mentionné {montantTotal}." });
                }

                await _dbVirement.CommitTransactionAsync();
                _logger.LogInformation("Virement en masse réussi. Total débité : {TotalDebite}, Frais : {Frais}", totalDébité, frais);

                return Ok(new
                {
                    message = "Virement en masse effectué avec succès.",
                    details = virementsEffectués,
                    frais = frais,
                    nombreVirements = idsVirements.Count
                });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du traitement du virement en masse.");
                return StatusCode(500, new { message = "Une erreur est survenue lors du virement en masse." });
            }
        }


        /*
        private async Task<IActionResult> VirementDeMasseTraitement(string fichier)
        {
            if (string.IsNullOrEmpty(fichier) || !System.IO.File.Exists(fichier))
            {
                _logger.LogWarning("Fichier introuvable : {FichierPath}", fichier);
                return BadRequest(new { message = "Fichier introuvable." });
            }

            int beneficiaireId;
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;

            if (clientId == null)
            {
                _logger.LogWarning("Utilisateur non authentifié.");
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            _logger.LogInformation("Début du traitement du virement en masse pour le fichier : {FichierPath}", fichier);

            string[] lines;
            try
            {
                lines = await System.IO.File.ReadAllLinesAsync(fichier);
                if (lines.Length < 4 || !lines[0].StartsWith("DEBUT FICHIER STB"))
                {
                    _logger.LogWarning("Format de fichier invalide.");
                    return BadRequest(new { message = "Format de fichier invalide." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du fichier.");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la lecture du fichier." });
            }

            var ribEmetteur = lines[1].Substring(1, 20).Trim();
            var montantTotalString = Regex.Match(lines[1], @"STB\s*(\d+)").Groups[1].Value;
            if (!decimal.TryParse(montantTotalString, out var montantTotal))
            {
                _logger.LogWarning("Le montant total n'est pas dans un format valide : {Montant}", montantTotalString);
                return BadRequest(new { message = "Le montant total n'est pas dans un format valide." });
            }

            // Récupération du compte émetteur
            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                _logger.LogWarning("Compte émetteur introuvable : {RIB}", ribEmetteur);
                return NotFound(new { message = "Compte émetteur introuvable." });
            }

            // Calcul du nombre de bénéficiaires
            int nombreBeneficiaires = lines.Length - 3; // -3 pour les lignes DEBUT, EMETTEUR et FIN

            // Calcul des frais
            decimal frais = 5.0m;
            if (nombreBeneficiaires > 5)
            {
                frais += (nombreBeneficiaires - 5) * 0.5m;
            }

            // Calcul du solde disponible (incluant le découvert autorisé)
            decimal soldeDisponible = emetteur.Solde + emetteur.DecouvertAutorise;

            // Vérification du solde disponible
            if (soldeDisponible < (montantTotal + frais))
            {
                _logger.LogWarning("Solde insuffisant pour l'émetteur {RIB}. Solde disponible : {SoldeDisponible}, Montant requis : {MontantTotal}, Frais : {Frais}",
                    ribEmetteur, soldeDisponible, montantTotal, frais);
                return BadRequest(new { message = $"Solde insuffisant, y compris avec le découvert autorisé. Montant nécessaire : {montantTotal + frais}" });
            }

            await _dbVirement.BeginTransactionAsync();
            _logger.LogInformation("Transaction de virement en masse commencée.");

            try
            {
                var idsVirements = new List<int>();
                decimal totalDébité = 0;
                var virementsEffectués = new List<Virement>();

                for (int i = 2; i < lines.Length - 1; i++)
                {
                    var beneficiaireLigne = lines[i].Trim();
                    var matchBenef = Regex.Match(beneficiaireLigne, @"^B(\d{20})(.+?)\s+(\d+)(\w+)$");

                    if (!matchBenef.Success)
                    {
                        _logger.LogWarning("Format du bénéficiaire invalide. Ligne reçue: {Ligne}", beneficiaireLigne);
                        return BadRequest(new { message = "Format du bénéficiaire invalide." });
                    }

                    var ribBeneficiaire = matchBenef.Groups[1].Value.Trim();
                    var montantStr = matchBenef.Groups[3].Value.Trim();
                    var motif = matchBenef.Groups[4].Value.Trim();

                    if (!decimal.TryParse(montantStr, out var montant))
                    {
                        _logger.LogWarning("Montant invalide pour le bénéficiaire {RIB} : {Montant}", ribBeneficiaire, montantStr);
                        return BadRequest(new { message = "Montant invalide dans le fichier." });
                    }

                    var beneficiaire = (await _dbCompte.GetAllAsync(c => c.RIB == ribBeneficiaire)).FirstOrDefault();
                    if (beneficiaire == null)
                    {
                        _logger.LogWarning("Compte bénéficiaire introuvable : {RIB}", ribBeneficiaire);
                        return NotFound(new { message = "Compte bénéficiaire introuvable." });
                    }

                    // Débit/Crédit en tenant compte du découvert
                    emetteur.Solde -= montant;
                    if (i == 2) // Premier virement, on déduit les frais
                    {
                        emetteur.Solde -= frais;
                    }
                    beneficiaire.Solde += montant;

                    // Ajustement du découvert autorisé si le solde devient négatif
                    if (emetteur.Solde < 0)
                    {
                        emetteur.DecouvertAutorise += emetteur.Solde;
                    }

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(beneficiaire);

                    var virement = new Virement
                    {
                        RIB_Emetteur = ribEmetteur,
                        RIB_Recepteur = ribBeneficiaire,
                        Montant = montant,
                        DateVirement = DateTime.Now,
                        Statut = "Réussi",
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = fichier,
                        Description = $"Virement de masse depuis {ribEmetteur}",
                        Motif = motif
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                    idsVirements.Add(virement.Id);
                    totalDébité += montant;
                }

                // Enregistrement des frais dans la table FraisCompte
                var fraisCompte = new FraisCompte
                {
                    RIB = ribEmetteur,
                    type = "Virement multiple",
                    Date = DateTime.Now,
                    Montant = frais,
                    IdsVirements = idsVirements
                };
                await _dbFraisCompte.CreateAsync(fraisCompte);

                if (totalDébité != montantTotal)
                {
                    return BadRequest(new { message = $"Total débité {totalDébité} différent du Montant à débiter Mentionné {montantTotal}." });
                }

                await _dbVirement.CommitTransactionAsync();
                _logger.LogInformation("Virement en masse réussi. Total débité : {TotalDebite}, Frais : {Frais}", totalDébité, frais);

                return Ok(new
                {
                    message = "Virement en masse effectué avec succès.",
                    details = virementsEffectués,
                    frais = frais,
                    nombreVirements = idsVirements.Count
                });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du traitement du virement en masse.");
                return StatusCode(500, new { message = "Une erreur est survenue lors du virement en masse." });
            }
        }
        */





        /*
        private async Task<IActionResult> VirementDeMasseTraitement(string fichier)
        {
            if (string.IsNullOrEmpty(fichier) || !System.IO.File.Exists(fichier))
            {
                _logger.LogWarning("Fichier introuvable : {FichierPath}", fichier);
                return BadRequest(new { message = "Fichier introuvable." });
            }
            int beneficiaireId;

            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;
            if (clientId == null)
            {
                _logger.LogWarning("Utilisateur non authentifié.");
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            _logger.LogInformation("Début du traitement du virement en masse pour le fichier : {FichierPath}", fichier);

            string[] lines;
            try
            {
                lines = await System.IO.File.ReadAllLinesAsync(fichier);
                if (lines.Length < 4 || !lines[0].StartsWith("DEBUT FICHIER STB"))
                {
                    _logger.LogWarning("Format de fichier invalide.");
                    return BadRequest(new { message = "Format de fichier invalide." });
                }

                var emetteurLigne = lines[1].Trim();
                var match = Regex.Match(emetteurLigne, @"^D(\d{20})STB\s*(\d+)\s*(\w+)?$");

                if (!match.Success)
                {
                    _logger.LogWarning("Format de l'émetteur invalide. Ligne reçue: {Ligne}", emetteurLigne);
                    return BadRequest(new { message = "Format de l'émetteur invalide." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du fichier.");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la lecture du fichier." });
            }

            var ribEmetteur = lines[1].Substring(1, 20).Trim();
            var montantTotalString = Regex.Match(lines[1], @"STB\s*(\d+)").Groups[1].Value;
            if (!decimal.TryParse(montantTotalString, out var montantTotal))
            {
                _logger.LogWarning("Le montant total n'est pas dans un format valide : {Montant}", montantTotalString);
                return BadRequest(new { message = "Le montant total n'est pas dans un format valide." });
            }

            // Récupération du compte émetteur
            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                _logger.LogWarning("Compte émetteur introuvable : {RIB}", ribEmetteur);
                return NotFound(new { message = "Compte émetteur introuvable." });
            }

            // Calcul du nombre de bénéficiaires
            int nombreBeneficiaires = lines.Length - 3; // -3 pour les lignes DEBUT, EMETTEUR et FIN

            // Calcul des frais
            decimal frais = 5.0m; // Frais de base pour les 5 premiers bénéficiaires
            if (nombreBeneficiaires > 5)
            {
                frais += (nombreBeneficiaires - 5) * 0.5m;
            }

            // Vérification du solde (montant total + frais)
            if (emetteur.Solde < (montantTotal + frais))
            {
                _logger.LogWarning("Solde insuffisant pour l'émetteur {RIB}, Solde : {Solde}, Montant requis : {Montant}, Frais : {Frais}",
                    ribEmetteur, emetteur.Solde, montantTotal, frais);
                return BadRequest(new { message = $"Solde insuffisant sur le compte émetteur. Montant nécessaire : {montantTotal + frais}" });
            }

            // Début de transaction
            await _dbVirement.BeginTransactionAsync();
            _logger.LogInformation("Transaction de virement en masse commencée.");

            try
            {
                var idsVirements = new List<int>(); //utiliser pour qtocker les ids des vireements de ce virements de masse dans la table FraisCompte
                decimal totalDébité = 0;
                var virementsEffectués = new List<Virement>();
                int idVirementMasse = 0; // Pour stocker l'ID du premier virement de masse

                for (int i = 2; i < lines.Length - 1; i++)
                {
                    var beneficiaireLigne = lines[i].Trim();
                    _logger.LogInformation("Ligne brute: {Ligne}", beneficiaireLigne);

                    var matchBenef = Regex.Match(beneficiaireLigne, @"^B(\d{20})(.+?)\s+(\d+)(\w+)$");

                    if (!matchBenef.Success)
                    {
                        _logger.LogWarning("Format du bénéficiaire invalide. Ligne reçue: {Ligne}", beneficiaireLigne);
                        return BadRequest(new { message = "Format du bénéficiaire invalide." });
                    }

                    var ribBeneficiaire = matchBenef.Groups[1].Value.Trim();
                    var nomBeneficiaire = matchBenef.Groups[2].Value.Trim();
                    var montantStr = matchBenef.Groups[3].Value.Trim();
                    var motif = matchBenef.Groups[4].Value.Trim();

                    _logger.LogInformation("RIB: {RIB}, Nom: {Nom}, Montant: {Montant}, Motif: {Motif}",
                                            ribBeneficiaire, nomBeneficiaire, montantStr, motif);

                    if (!decimal.TryParse(montantStr, out var montant))
                    {
                        _logger.LogWarning("Montant invalide pour le bénéficiaire {RIB} : {Montant}", ribBeneficiaire, montantStr);
                        return BadRequest(new { message = "Montant invalide dans le fichier." });
                    }

                    // Vérifier si le compte bénéficiaire existe
                    var beneficiaire = (await _dbCompte.GetAllAsync(c => c.RIB == ribBeneficiaire)).FirstOrDefault();
                    if (beneficiaire == null)
                    {
                        _logger.LogWarning("Compte bénéficiaire introuvable : {RIB}", ribBeneficiaire);
                        return NotFound(new { message = "Compte bénéficiaire introuvable." });
                    }

                    // Vérifier si le bénéficiaire est déjà enregistré pour ce client
                    var beneficiaireExistant = await _dbBeneficiaire.GetAllAsync(b =>
                        b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                    if (!beneficiaireExistant.Any())
                    {
                        // Enregistrer le nouveau bénéficiaire
                        var nouveauBeneficiaire = new Beneficiaire
                        {
                            ClientId = clientId,
                            Nom = nomBeneficiaire,
                            RIBCompte = ribBeneficiaire,
                            Prenom = nomBeneficiaire,
                        };

                        await _dbBeneficiaire.CreateAsync(nouveauBeneficiaire);
                        _logger.LogInformation("Nouveau bénéficiaire enregistré: {Nom}, RIB: {RIB}", nomBeneficiaire, ribBeneficiaire);

                        // On recharge le bénéficiaire pour obtenir son ID généré
                        var beneficiaireCree = await _dbBeneficiaire.GetAllAsync(b =>
                            b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                        beneficiaireId = beneficiaireCree.First().Id;
                    }
                    else
                    {
                        beneficiaireId = beneficiaireExistant.First().Id;
                    }

                    // Vérifier que l'émetteur a assez de fonds (montant + frais)
                    if (emetteur.Solde < (montant + (i == 2 ? frais : 0))) // On applique les frais seulement pour le premier virement
                    {
                        _logger.LogWarning("Solde insuffisant pour l'émetteur {RIB}, Solde : {Solde}, Montant requis : {Montant}, Frais : {Frais}",
                            ribEmetteur, emetteur.Solde, montant, i == 2 ? frais : 0);
                        return BadRequest(new { message = "Solde insuffisant sur le compte émetteur." });
                    }

                    // Mise à jour des soldes
                    emetteur.Solde -= montant;
                    if (i == 2) // On déduit les frais seulement une fois, avec le premier virement
                    {
                        emetteur.Solde -= frais;
                    }
                    beneficiaire.Solde += montant;
                    totalDébité += montant;

                    _logger.LogInformation("Virement de {Montant} effectué de {RIBEmetteur} vers {RIBBeneficiaire}. Solde restant : {SoldeEmetteur}",
                        montant, ribEmetteur, ribBeneficiaire, emetteur.Solde);

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(beneficiaire);

                    var virement = new Virement
                    {
                        RIB_Emetteur = ribEmetteur,
                        RIB_Recepteur = ribBeneficiaire,
                        Montant = montant,
                        DateVirement = DateTime.Now,
                        Statut = "Réussi",
                        Motif = motif,
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = fichier,
                        Description = $"Virement de masse depuis {ribEmetteur}"
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                    idsVirements.Add(virement.Id);

                    // On garde l'ID du premier virement pour l'enregistrement des frais
                    if (i == 2)
                    {
                        idVirementMasse = virement.Id;
                    }
                }

                // Enregistrement des frais dans la table FraisCompte
                var fraisCompte = new FraisCompte
                {
                    RIB = ribEmetteur,
                    type = "Virement multiple",
                    Date = DateTime.Now,
                    Montant = frais,
                    //IdVirement = idVirementMasse
                    IdsVirements = idsVirements // Utilisation de la propriété NotMapped
                };
                await _dbFraisCompte.CreateAsync(fraisCompte);

                if (totalDébité != montantTotal)
                {
                    return BadRequest(new { message = $"Total débité {totalDébité} différent du Montant à débiter Mentionné {montantTotal}." });
                }

                await _dbVirement.CommitTransactionAsync();
                _logger.LogInformation("Virement en masse réussi. Total débité : {TotalDebite}, Frais : {Frais}", totalDébité, frais);

                return Ok(new
                {
                    message = "Virement en masse effectué avec succès.",
                    details = virementsEffectués,
                    frais = frais,
                    nombreVirements = idsVirements.Count
                });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du traitement du virement en masse.");
                return StatusCode(500, new { message = "Une erreur est survenue lors du virement en masse." });
            }
        }*/

        /*

        private async Task<IActionResult> VirementDeMasseTraitement(string fichier)
        {
            if (string.IsNullOrEmpty(fichier) || !System.IO.File.Exists(fichier))
            {
                _logger.LogWarning("Fichier introuvable : {FichierPath}", fichier);
                return BadRequest(new { message = "Fichier introuvable." });
            }
            int beneficiaireId;

            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;
            if (clientId == null)
            {
                _logger.LogWarning("Utilisateur non authentifié.");
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            _logger.LogInformation("Début du traitement du virement en masse pour le fichier : {FichierPath}", fichier);

            string[] lines;
            try
            {
                lines = await System.IO.File.ReadAllLinesAsync(fichier);
                if (lines.Length < 4 || !lines[0].StartsWith("DEBUT FICHIER STB"))
                {
                    _logger.LogWarning("Format de fichier invalide.");
                    return BadRequest(new { message = "Format de fichier invalide." });
                }

                var emetteurLigne = lines[1].Trim();
                var match = Regex.Match(emetteurLigne, @"^D(\d{20})STB\s*(\d+)\s*(\w+)?$");

                if (!match.Success)
                {
                    _logger.LogWarning("Format de l'émetteur invalide. Ligne reçue: {Ligne}", emetteurLigne);
                    return BadRequest(new { message = "Format de l'émetteur invalide." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du fichier.");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la lecture du fichier." });
            }

            var ribEmetteur = lines[1].Substring(1, 20).Trim();
            var montantTotalString = Regex.Match(lines[1], @"STB\s*(\d+)").Groups[1].Value;
            if (!decimal.TryParse(montantTotalString, out var montantTotal))
            {
                _logger.LogWarning("Le montant total n'est pas dans un format valide : {Montant}", montantTotalString);
                return BadRequest(new { message = "Le montant total n'est pas dans un format valide." });
            }

            // Récupération du compte émetteur
            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                _logger.LogWarning("Compte émetteur introuvable : {RIB}", ribEmetteur);
                return NotFound(new { message = "Compte émetteur introuvable." });
            }

            if (emetteur.Solde < montantTotal)
            {
                _logger.LogWarning("Solde insuffisant pour l'émetteur {RIB}, Solde : {Solde}, Montant requis : {Montant}",
                    ribEmetteur, emetteur.Solde, montantTotal);
                return BadRequest(new { message = "Solde insuffisant sur le compte émetteur." });
            }

            // Début de transaction
            await _dbVirement.BeginTransactionAsync();
            _logger.LogInformation("Transaction de virement en masse commencée.");

            try
            {
                decimal totalDébité = 0;
                var virementsEffectués = new List<Virement>();

                for (int i = 2; i < lines.Length - 1; i++)
                {
                    var beneficiaireLigne = lines[i].Trim();
                    _logger.LogInformation("Ligne brute: {Ligne}", beneficiaireLigne);

                    var matchBenef = Regex.Match(beneficiaireLigne, @"^B(\d{20})(.+?)\s+(\d+)(\w+)$");

                    if (!matchBenef.Success)
                    {
                        _logger.LogWarning("Format du bénéficiaire invalide. Ligne reçue: {Ligne}", beneficiaireLigne);
                        return BadRequest(new { message = "Format du bénéficiaire invalide." });
                    }

                    var ribBeneficiaire = matchBenef.Groups[1].Value.Trim();
                    var nomBeneficiaire = matchBenef.Groups[2].Value.Trim();
                    var montantStr = matchBenef.Groups[3].Value.Trim();
                    var motif = matchBenef.Groups[4].Value.Trim();

                    _logger.LogInformation("RIB: {RIB}, Nom: {Nom}, Montant: {Montant}, Motif: {Motif}",
                                            ribBeneficiaire, nomBeneficiaire, montantStr, motif);

                    if (!decimal.TryParse(montantStr, out var montant))
                    {
                        _logger.LogWarning("Montant invalide pour le bénéficiaire {RIB} : {Montant}", ribBeneficiaire, montantStr);
                        return BadRequest(new { message = "Montant invalide dans le fichier." });
                    }

                    // Vérifier si le compte bénéficiaire existe
                    var beneficiaire = (await _dbCompte.GetAllAsync(c => c.RIB == ribBeneficiaire)).FirstOrDefault();
                    if (beneficiaire == null)
                    {
                        _logger.LogWarning("Compte bénéficiaire introuvable : {RIB}", ribBeneficiaire);
                        return NotFound(new { message = "Compte bénéficiaire introuvable." });
                    }

                    // Vérifier si le bénéficiaire est déjà enregistré pour ce client
                    var beneficiaireExistant = await _dbBeneficiaire.GetAllAsync(b =>
                        b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                    if (!beneficiaireExistant.Any())
                    {
                        // Enregistrer le nouveau bénéficiaire
                        var nouveauBeneficiaire = new Beneficiaire
                        {
                            ClientId = clientId,
                            Nom = nomBeneficiaire,
                            RIBCompte = ribBeneficiaire,
                           Prenom= nomBeneficiaire,
                        };

                        await _dbBeneficiaire.CreateAsync(nouveauBeneficiaire);
                        _logger.LogInformation("Nouveau bénéficiaire enregistré: {Nom}, RIB: {RIB}", nomBeneficiaire, ribBeneficiaire);


                        // On recharge le bénéficiaire pour obtenir son ID généré
                        var beneficiaireCree = await _dbBeneficiaire.GetAllAsync(b =>
                            b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                        beneficiaireId = beneficiaireCree.First().Id;
                    }

                    else
                    {
                        beneficiaireId = beneficiaireExistant.First().Id;
                    }

                    // Vérifier que l'émetteur a assez de fonds
                    if (emetteur.Solde < montant)
                    {
                        _logger.LogWarning("Solde insuffisant pour l'émetteur {RIB}, Solde : {Solde}, Montant requis : {Montant}",
                            ribEmetteur, emetteur.Solde, montant);
                        return BadRequest(new { message = "Solde insuffisant sur le compte émetteur." });
                    }

                    // Mise à jour des soldes
                    emetteur.Solde -= montant;
                    beneficiaire.Solde += montant;
                    totalDébité += montant;

                    _logger.LogInformation("Virement de {Montant} effectué de {RIBEmetteur} vers {RIBBeneficiaire}. Solde restant : {SoldeEmetteur}",
                        montant, ribEmetteur, ribBeneficiaire, emetteur.Solde);

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(beneficiaire);

                    var virement = new Virement
                    {
                        RIB_Emetteur = ribEmetteur,
                        RIB_Recepteur = ribBeneficiaire,
                        Montant = montant,
                        DateVirement = DateTime.Now,
                        Statut = "Réussi",
                        Motif = motif,
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = fichier,
                        Description = $"Virement de masse depuis {ribEmetteur}"
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                }

                if (totalDébité != montantTotal)
                {
                    return BadRequest(new { message = $"Total débité {totalDébité} différent du Montant à débiter Mentionné {montantTotal}." });
                }

                await _dbVirement.CommitTransactionAsync();
                _logger.LogInformation("Virement en masse réussi. Total débité : {TotalDebite}", totalDébité);

                return Ok(new { message = "Virement en masse effectué avec succès.", details = virementsEffectués });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du traitement du virement en masse.");
                return StatusCode(500, new { message = "Une erreur est survenue lors du virement en masse." });
            }
        }*/

        /*
        private async Task<IActionResult> VirementDeMasseTraitement(string fichier)
        {
            // Vérification du fichier
            if (string.IsNullOrEmpty(fichier) || !System.IO.File.Exists(fichier))
            {
                _logger.LogWarning("Fichier introuvable : {FichierPath}", fichier);
                return BadRequest(new { message = "Fichier introuvable." });
            }

            // Authentification
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;
            if (clientId == null)
            {
                _logger.LogWarning("Utilisateur non authentifié.");
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            _logger.LogInformation("Début du traitement du virement en masse pour le fichier : {FichierPath}", fichier);

            // Lecture du fichier
            string[] lines;
            try
            {
                lines = await System.IO.File.ReadAllLinesAsync(fichier);
                if (lines.Length < 4 || !lines[0].StartsWith("DEBUT FICHIER STB"))
                {
                    _logger.LogWarning("Format de fichier invalide.");
                    return BadRequest(new { message = "Format de fichier invalide." });
                }

                var emetteurLigne = lines[1].Trim();
                var match = Regex.Match(emetteurLigne, @"^D(\d{20})STB\s*(\d+)\s*(\w+)?$");

                if (!match.Success)
                {
                    _logger.LogWarning("Format de l'émetteur invalide. Ligne reçue: {Ligne}", emetteurLigne);
                    return BadRequest(new { message = "Format de l'émetteur invalide." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du fichier.");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la lecture du fichier." });
            }

            // Extraction des informations de l'émetteur
            var ribEmetteur = lines[1].Substring(1, 20).Trim();
            var montantTotalString = Regex.Match(lines[1], @"STB\s*(\d+)").Groups[1].Value;
            if (!decimal.TryParse(montantTotalString, out var montantTotal))
            {
                _logger.LogWarning("Le montant total n'est pas dans un format valide : {Montant}", montantTotalString);
                return BadRequest(new { message = "Le montant total n'est pas dans un format valide." });
            }

            // Récupération du compte émetteur
            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == ribEmetteur)).FirstOrDefault();
            if (emetteur == null)
            {
                _logger.LogWarning("Compte émetteur introuvable : {RIB}", ribEmetteur);
                return NotFound(new { message = "Compte émetteur introuvable." });
            }

            // Calcul des frais
            int nombreBeneficiaires = lines.Length - 3;
            decimal frais = 5.0m;
            if (nombreBeneficiaires > 5)
            {
                frais += 0.5m * (nombreBeneficiaires - 5);
            }

            // Vérification du solde
            decimal totalADebiter = montantTotal + frais;
            if (emetteur.Solde < totalADebiter)
            {
                _logger.LogWarning("Solde insuffisant. Solde: {Solde}, Total à débiter: {Total}", emetteur.Solde, totalADebiter);
                return BadRequest(new { message = $"Solde insuffisant. Montant nécessaire: {totalADebiter}" });
            }

            // Début de transaction
            await _dbVirement.BeginTransactionAsync();
            _logger.LogInformation("Transaction de virement en masse commencée.");

            try
            {
                decimal totalDébité = 0;
                var virementsEffectués = new List<Virement>();

                // Création du virement parent
                var virementParent = new Virement
                {
                    RIB_Emetteur = ribEmetteur,
                    RIB_Recepteur = "MULTIPLE", // Valeur spéciale pour les virements multiples
                    Montant = montantTotal,
                    DateVirement = DateTime.Now,
                    Statut = "En cours",
                    Motif = "Virement de masse",
                    TypeVirement = "VirementDeMasse",
                    FichierBeneficaires = fichier,
                    Description = $"Virement de masse - {nombreBeneficiaires} bénéficiaires"
                };
                await _dbVirement.CreateAsync(virementParent);

                // Enregistrement des frais
                var fraisCompte = new FraisCompte
                {
                    RIB = ribEmetteur,
                    IdVirement = virementParent.Id,
                    Montant = frais,
                    type = "Virement multiple",
                    Date = DateTime.Now
                };
                await _dbFraisCompte.CreateAsync(fraisCompte);

                // Débit des frais
                emetteur.Solde -= frais;
                await _dbCompte.UpdateAsync(emetteur);

                // Traitement des bénéficiaires
                for (int i = 2; i < lines.Length - 1; i++)
                {
                    var beneficiaireLigne = lines[i].Trim();
                    var matchBenef = Regex.Match(beneficiaireLigne, @"^B(\d{20})(.+?)\s+(\d+)(\w+)$");

                    if (!matchBenef.Success)
                    {
                        _logger.LogWarning("Format du bénéficiaire invalide. Ligne: {Ligne}", beneficiaireLigne);
                        continue; // On passe au bénéficiaire suivant
                    }

                    var ribBeneficiaire = matchBenef.Groups[1].Value.Trim();
                    var nomBeneficiaire = matchBenef.Groups[2].Value.Trim();
                    var montantStr = matchBenef.Groups[3].Value.Trim();
                    var motif = matchBenef.Groups[4].Value.Trim();

                    if (!decimal.TryParse(montantStr, out var montant))
                    {
                        _logger.LogWarning("Montant invalide pour le bénéficiaire {RIB}", ribBeneficiaire);
                        continue;
                    }

                    // Vérification du compte bénéficiaire
                    var beneficiaire = (await _dbCompte.GetAllAsync(c => c.RIB == ribBeneficiaire)).FirstOrDefault();
                    if (beneficiaire == null)
                    {
                        _logger.LogWarning("Compte bénéficiaire introuvable: {RIB}", ribBeneficiaire);
                        continue;
                    }

                    // Gestion du bénéficiaire
                    var beneficiaireExistant = await _dbBeneficiaire.GetAllAsync(b =>
                        b.ClientId == clientId && b.RIBCompte == ribBeneficiaire);

                    int beneficiaireId;
                    if (!beneficiaireExistant.Any())
                    {
                        var nouveauBeneficiaire = new Beneficiaire
                        {
                            ClientId = clientId,
                            Nom = nomBeneficiaire,
                            Prenom = nomBeneficiaire,
                            RIBCompte = ribBeneficiaire
                        };
                        await _dbBeneficiaire.CreateAsync(nouveauBeneficiaire);
                        beneficiaireId = nouveauBeneficiaire.Id;
                    }
                    else
                    {
                        beneficiaireId = beneficiaireExistant.First().Id;
                    }

                    // Exécution du virement
                    emetteur.Solde -= montant;
                    beneficiaire.Solde += montant;
                    totalDébité += montant;

                    await _dbCompte.UpdateAsync(emetteur);
                    await _dbCompte.UpdateAsync(beneficiaire);

                    var virement = new Virement
                    {
                        RIB_Emetteur = ribEmetteur,
                        RIB_Recepteur = ribBeneficiaire,
                        Montant = montant,
                        DateVirement = DateTime.Now,
                        Statut = "Réussi",
                        Motif = motif,
                        TypeVirement = "VirementDeMasse",
                        FichierBeneficaires = fichier,
                        Description = $"Virement depuis {ribEmetteur}",
                       Id = virementParent.Id
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                }

                // Validation finale
                if (totalDébité != montantTotal)
                {
                    _logger.LogWarning("Incohérence de montant. Total débité: {Total}, Montant attendu: {Attendu}",
                        totalDébité, montantTotal);
                    return BadRequest(new { message = "Incohérence dans les montants débités." });
                }

                // Mise à jour du virement parent
                virementParent.Statut = "Réussi";
                virementParent.Montant = totalDébité;
                await _dbVirement.UpdateAsync(virementParent);

                await _dbVirement.CommitTransactionAsync();

                _logger.LogInformation("Virement en masse réussi. Total: {Total}, Frais: {Frais}, Bénéficiaires: {Nb}",
                    totalDébité, frais, nombreBeneficiaires);

                return Ok(new
                {
                    success = true,
                    message = "Virement en masse effectué avec succès.",
                    total = totalDébité,
                    frais = frais,
                    nombreBeneficiaires = nombreBeneficiaires,
                    virementParentId = virementParent.Id
                });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du virement en masse");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Une erreur est survenue lors du virement en masse.",
                    details = ex.Message
                });
            }
        }



*/

























        /*[HttpPost("BlocageRetrait")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BlocageRetrait([FromBody] string rib)
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var compte = (await _dbCompte.GetAllAsync(c => c.RIB == rib && c.ClientId == clientId)).FirstOrDefault();
            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable ou non autorisé." });
            }
            if (compte.Type.ToLower() == "epargne")
            {
                return BadRequest(new { message = "Seuls les comptes épargne peuvent être bloqués." });
            }

            compte.Statut = "retrait bloqué";
            await _dbCompte.UpdateAsync(compte);

            return Ok(new { message = "Retrait bloqué avec succès." });
        }
*/
        /*[HttpPost("DeblocageRetrait")]
      
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeblocageRetrait([FromBody] string rib)
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var compte = (await _dbCompte.GetAllAsync(c => c.RIB == rib && c.ClientId == clientId)).FirstOrDefault();
            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable ou non autorisé." });
            }

            if (compte.Type.ToLower() == "epargne" || compte.Statut != "retrait bloqué")
            {
                return BadRequest(new { message = "Le compte doit être un compte épargne avec retrait bloqué pour être débloqué." });
            }

            compte.Statut = "actif";
            await _dbCompte.UpdateAsync(compte);

            return Ok(new { message = "Retrait débloqué avec succès." });
        }
        */











































        private int GetUserIdFromToken()
        {
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader))
                {
                    throw new UnauthorizedAccessException("Header Authorization manquant");
                }

                var tokenParts = authHeader.Split(' ');
                if (tokenParts.Length != 2 || !tokenParts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Format d'autorisation invalide");
                }

                var token = tokenParts[1].Trim();
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    throw new UnauthorizedAccessException("Le token n'est pas un JWT valide");
                }

                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Sub ||
                    c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    throw new UnauthorizedAccessException("Claim d'identifiant utilisateur invalide");
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetUserIdFromToken");
                throw new UnauthorizedAccessException("Erreur de traitement du token", ex);
            }
        }


        /* private int? GetClientIdFromToken()
         {
             var identity = HttpContext.User.Identity as ClaimsIdentity;
             if (identity != null)
             {
                 var clientIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                 if (clientIdClaim != null)
                 {
                     return int.Parse(clientIdClaim.Value);
                 }
             }
             return null;
         }*/



        [HttpGet("historiqueVirements/{rib}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistoriqueVirements(string rib, string filter = "all")
        {
            try
            {
                IEnumerable<Virement> virementsEnvoyes = new List<Virement>();
                IEnumerable<Virement> virementsRecus = new List<Virement>();

                if (filter == "all" || filter == "sent")
                {
                    virementsEnvoyes = await _dbVirement.GetAllAsync(v => v.RIB_Emetteur == rib);
                }

                if (filter == "all" || filter == "received")
                {
                    virementsRecus = await _dbVirement.GetAllAsync(v => v.RIB_Recepteur == rib);
                }

                var historiqueVirements = new
                {
                    VirementsEnvoyes = virementsEnvoyes.Select(v => new
                    {
                        Date = v.DateVirement,
                        Montant = v.Montant,
                        Motif = v.Motif,
                        RIB_Recepteur = v.RIB_Recepteur
                    }),
                    VirementsRecus = virementsRecus.Select(v => new
                    {
                        Date = v.DateVirement,
                        Montant = v.Montant,
                        Motif = v.Motif,
                        RIB_Emetteur = v.RIB_Emetteur
                    })
                };

                if (!virementsEnvoyes.Any() && !virementsRecus.Any())
                {
                    return NotFound(new { message = "Aucun virement trouvé pour ce compte." });
                }

                return Ok(historiqueVirements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique des virements.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Une erreur est survenue lors de la récupération de l'historique des virements." });
            }
        }





    }
}