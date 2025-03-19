using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;

using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APICompte.Repository.IRepository;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using STBEverywhere_back_APICompte.Services;

namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/CompteApi")]
    [ApiController]
    public class CompteAPIController : ControllerBase
    {

        private readonly ICompteService _compteService;

        //private readonly ICompteRepository _dbCompte;
        private readonly IVirementRepository _dbVirement;


        private readonly ILogger<CompteAPIController> _logger;
        private readonly IMapper _mapper;
        public CompteAPIController(ICompteService compteService /*ICompteRepository dbCompte*/, IVirementRepository dbVirement, ILogger<CompteAPIController> logger, IMapper mapper)
        {
            _compteService = compteService;
            //_dbCompte = dbCompte;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
        }



        [HttpGet("listecompte")]
        [Authorize]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetComptesByClientId()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            //var comptes = await _context.Compte
            // var comptes = await _dbCompte.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé");

            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé");
            if (comptes == null || !comptes.Any())
            {
                return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
            }
            _logger.LogInformation("Getting all comptes");
            return Ok(comptes);
        }

        /* public async Task<IActionResult> GetComptesByCin(string numCin)
         {
             var clientId = GetClientIdFromToken();
             if (clientId == null)
             {
                 return Unauthorized(new { message = "Utilisateur non authentifié" });
             }
             //var comptes = await _context.Compte
             var comptes = await _dbCompte.GetAllAsync(c => c.NumCin == numCin && c.Statut != "Clôturé");
             /*.Where(c => c.NumCin == numCin && c.statut != "Clôturé")
             .Select(c => new
             {
                 c.RIB,
                 c.type,
                 c.solde
             })
             .ToListAsync();

             if (comptes == null || !comptes.Any())
             {
                 return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
             }
             _logger.LogInformation("Getting all comptes");
             return Ok(comptes);
         }*/
        //les comptes qui peuvent effectuer des virements 
        [HttpGet("listecompteVirement")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetComptesVirementByClientId()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            // Exclure les comptes avec statut "Clôturé" et les comptes d'épargne
            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé" && c.Type.ToLower()!= "epargne");

            if (comptes == null || !comptes.Any())
            {
                return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
            }

            _logger.LogInformation("Récupération des comptes actifs non épargne réussie.");
            return Ok(comptes);
        }

        [HttpPost("CreateCompte")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        public async Task<IActionResult> CreateCompte([FromBody] CreateCompteDto compteDto)
         {
            var clientId = GetClientIdFromToken();
            _logger.LogInformation($"ClientId récupéré depuis le token : {clientId}");

            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            if (compteDto == null ||  string.IsNullOrEmpty(compteDto.type))
             {
                 _logger.LogError(" Type  obligatoire");
                 return BadRequest(new { message = "Type  obligatoires." });
            }
            var clientList = await _compteService.GetAllAsync(c => c.ClientId == clientId );
            _logger.LogInformation($"Nombre de clients trouvés : {clientList.Count()}");

            var client = clientList.FirstOrDefault();
            _logger.LogInformation($"Client trouvé ? {(client != null ? "Oui" : "Non")}");

            /*if (client == null)
            {
                return BadRequest(new { message = "Aucun client trouvé avec ce NumCin." });
            }*/
            if (compteDto.type.ToLower() == "epargne")
             {
                 //var epargneCount = (await _dbCompte.GetAllAsync(c => c.NumCin == compteDto.NumCin && c.Type.ToLower() == "epargne")).Count;

                var epargneCount = (await _compteService.GetAllAsync(c => c.Type.ToLower() == "epargne")).Count;
                if (epargneCount >= 3)
                 {
                     return BadRequest(new { message = "Vous ne pouvez pas avoir plus de 3 comptes d'épargne." });
                 }
             }

             string generatedRIB = _compteService.GenerateUniqueRIB();
             decimal initialSolde = compteDto.type.ToLower() == "epargne" ? 10 : 0;
             // Utilisation d'AutoMapper pour convertir compteDto en Compte
             var compte = _mapper.Map<Compte>(compteDto);

             // Ajout des valeurs manquantes
             compte.RIB = generatedRIB;
             compte.Solde = initialSolde;
             compte.DateCreation = DateTime.Now;
             compte.Statut = "Actif";
             compte.ClientId = (int)clientId;
            compte.NumCin = client.NumCin;
            compte.NbrOperationsAutoriseesParJour = "illimité";
            compte.MontantMaxAutoriseParJour = 2000.000m;


            await _compteService.CreateAsync(compte);

            /* await _dbCompte.CreateAsync(compte);
             await _dbCompte.SaveAsync();*/
            //_context.Compte.Add(compte);
            // await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompteByRIB), new { rib = compte.RIB }, compte);
         }






        

        [HttpGet("GetByRIB/{rib}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompteByRIB(string rib)
        {
           
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            var compte = await _compteService.GetAllAsync(c => c.RIB == rib);
           


            if (compte == null || !compte.Any())
            {
                return NotFound(new { message = "Compte introuvable." });
            }

            return Ok(compte);
        }


        [HttpPut("Cloturer/{rib}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloturerCompte(string rib)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            var compte = ((await _compteService.GetAllAsync(c => c.RIB == rib)).FirstOrDefault());
            //var compte = await _context.Compte.FirstOrDefaultAsync(c => c.RIB == rib);

            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable." });
            }

            if (compte.Solde != 0)
            {
                ModelState.AddModelError("", "Vous devez mettre votre compte à zéro puis réessayer de le clôturer.");
                return BadRequest(ModelState);
            }

            compte.Statut = "Clôturé";
            //await _context.SaveChangesAsync();
            await _compteService.SaveAsync();
            return Ok(new { message = "Le compte a été clôturé avec succès." });
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

        [HttpGet("GetSoldeByRIB/{rib}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSoldeByRIB(string rib)
        {
            try
            {
                var solde = await _compteService.GetSoldeByRIBAsync(rib);
                return Ok(new { Solde = solde });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }




    }







}
