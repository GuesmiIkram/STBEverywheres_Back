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


namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/virement")]
    [ApiController]
    public class VirementApiController : ControllerBase
    {

        private readonly ICompteRepository _dbCompte;
        private readonly IVirementRepository _dbVirement;

        private readonly ILogger<VirementApiController> _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IVirementService _virementService;


        public VirementApiController(/*VirementService virementService,*/ IWebHostEnvironment webHostEnvironment,ICompteRepository dbCompte, IVirementRepository dbVirement, ILogger<VirementApiController> logger, IMapper mapper)
        {
            _dbCompte = dbCompte;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
           // _virementService = virementService;

        }

        [HttpPost("Virement")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Virement([FromBody] VirementUnitaireDto virementDto)
        {
            _logger.LogInformation("Requête reçue pour un virement. Données : {@virementDto}", virementDto);

            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            _logger.LogInformation("Début du traitement du virement.");

            if (virementDto == null || string.IsNullOrEmpty(virementDto.RIB_Emetteur) ||
                string.IsNullOrEmpty(virementDto.RIB_Recepteur) || virementDto.Montant <= 0)
            {
                _logger.LogWarning("Paramètres invalides : RIB émetteur, RIB récepteur ou montant manquants ou incorrects.");
                return BadRequest(new { message = "RIB émetteur, RIB récepteur et montant sont obligatoires et le montant doit être positif." });
            }

            var emetteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Emetteur)).FirstOrDefault();
            var recepteur = (await _dbCompte.GetAllAsync(c => c.RIB == virementDto.RIB_Recepteur)).FirstOrDefault();

            if (emetteur == null || recepteur == null)
            {
                _logger.LogWarning("Compte émetteur ou récepteur introuvable. RIB émetteur : {RIB_Emetteur}, RIB récepteur : {RIB_Recepteur}", virementDto.RIB_Emetteur, virementDto.RIB_Recepteur);
                return NotFound(new { message = "Compte émetteur ou récepteur introuvable." });
            }

            if (emetteur.Solde < virementDto.Montant)
            {
                _logger.LogWarning("Solde insuffisant sur le compte émetteur : {RIB_Emetteur}, solde disponible : {SoldeEmetteur}, montant demandé : {Montant}",
                    virementDto.RIB_Emetteur, emetteur.Solde, virementDto.Montant);
                return BadRequest(new { message = "Solde insuffisant sur le compte émetteur." });
            }

            await _dbVirement.BeginTransactionAsync();
            _logger.LogInformation("Transaction commencée.");

            try
            {
                _logger.LogInformation("Mise à jour des soldes des comptes.");
                emetteur.Solde -= virementDto.Montant;
                recepteur.Solde += virementDto.Montant;

                await _dbCompte.UpdateAsync(emetteur);
                await _dbCompte.UpdateAsync(recepteur);

                var virement = new Virement
                {
                    RIB_Emetteur = virementDto.RIB_Emetteur,
                    RIB_Recepteur = virementDto.RIB_Recepteur,
                    Montant = virementDto.Montant,
                    DateVirement = DateTime.Now,
                    Statut = "Réussi",
                    Motif = virementDto.motif,
                    TypeVirement = "Virement Unitaire",
                    FichierBeneficaires =null,

                    Description = virementDto.Description
                };

                await _dbVirement.CreateAsync(virement);
                await _dbVirement.CommitTransactionAsync();
                _logger.LogInformation("Virement effectué avec succès. RIB émetteur : {RIB_Emetteur}, RIB récepteur : {RIB_Recepteur}, montant : {Montant}",
                    virementDto.RIB_Emetteur, virementDto.RIB_Recepteur, virementDto.Montant);

                return Ok(new { message = "Virement effectué avec succès." });
            }
            catch (Exception ex)
            {
                await _dbVirement.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du traitement du virement.");
                return StatusCode(500, new { message = "Une erreur est survenue lors du virement." });
            }
        }




        //[HttpPost("UploadFichier")]

        [HttpPost("VirementDeMasse")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> VirementDeMasse([FromForm] UploadFichierRequest request)
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


        /*public async Task<IActionResult> UploadFichier([FromForm] UploadFichierRequest request)
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

        /*[HttpPost("VirementDeMasse")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]*/



        private async Task<IActionResult> VirementDeMasseTraitement(string fichier)
        {
            /* if (string.IsNullOrEmpty(fichier) || !System.IO.File.Exists(fichier))
             {
                 _logger.LogWarning("Fichier introuvable : {FichierPath}", fichier);
                 return BadRequest(new { message = "Fichier introuvable." });
             }*/

            var clientId = GetClientIdFromToken();
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

            var ribEmetteur = lines[1].Substring(1, 20).Trim(); // Extraction du RIB

            var montantTotal = 0m;


            var montantTotalString = Regex.Match(lines[1], @"STB\s*(\d+)").Groups[1].Value;
            if (!decimal.TryParse(montantTotalString, out montantTotal))
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

                    // Conversion du montant en decimal
                    if (!decimal.TryParse(montantStr, out var montant))
                    {
                        _logger.LogWarning("Montant invalide pour le bénéficiaire {RIB} : {Montant}", ribBeneficiaire, montantStr);
                        return BadRequest(new { message = "Montant invalide dans le fichier." });
                    }

                    _logger.LogInformation("RIB bénéficiaire : {RIB}, Nom : {Nom}, Montant : {Montant}",
                                            ribBeneficiaire, nomBeneficiaire, montant);

                    // Vérifier si le bénéficiaire existe
                    var beneficiaire = (await _dbCompte.GetAllAsync(c => c.RIB == ribBeneficiaire)).FirstOrDefault();
                    if (beneficiaire == null)
                    {
                        _logger.LogWarning("Compte bénéficiaire introuvable : {RIB}", ribBeneficiaire);
                        return NotFound(new { message = "Compte bénéficiaire introuvable." });
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
                        TypeVirement = "Virement en masse",
                        FichierBeneficaires = fichier,
                        Description = $"Virement de masse depuis {ribEmetteur}"
                    };

                    await _dbVirement.CreateAsync(virement);
                    virementsEffectués.Add(virement);
                }

                if (totalDébité != montantTotal)
                {
                    return BadRequest(new { message = "Total débité {totalDébité} différent du  Montant à débiter Mentionné  {montantTotal}." });

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
        }

        [HttpPost("BlocageRetrait")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BlocageRetrait([FromBody] string rib)
        {
            var clientId = GetClientIdFromToken();
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

        [HttpPost("DeblocageRetrait")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeblocageRetrait([FromBody] string rib)
        {
            var clientId = GetClientIdFromToken();
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






        private int? GetClientIdFromToken()
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
        }
    }



   /* [HttpGet("HistoriqueVirementsEnvoyes/{RIB_Emetteur}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HistoriqueVirementsEnvoyes(string RIB_Emetteur)
        {
            _logger.LogInformation("Requête reçue pour l'historique des virements envoyés. RIB émetteur : {RIB_Emetteur}", RIB_Emetteur);

            // Vérification si le RIB émetteur est valide
            if (string.IsNullOrEmpty(RIB_Emetteur))
            {
                return BadRequest(new { message = "Le RIB émetteur est obligatoire." });
            }

            // Récupération de tous les virements où le RIB émetteur correspond
            var virements = await _dbVirement.GetAllAsync(v => v.RIB_Emetteur == RIB_Emetteur);

            if (virements == null || !virements.Any())
            {
                _logger.LogWarning("Aucun virement trouvé pour le RIB émetteur : {RIB_Emetteur}", RIB_Emetteur);
                return NotFound(new { message = "Aucun virement trouvé pour ce RIB émetteur." });
            }

            // Sélection des données pertinentes à retourner
            var result = virements.Select(v => new
            {
                v.RIB_Recepteur,
                v.Montant,
                v.DateVirement,
                v.Motif,
                v.Description
            }).ToList();

            _logger.LogInformation("Historique des virements envoyés récupéré avec succès. Nombre de virements : {Count}", result.Count);

            return Ok(result);
        }*/



    }
