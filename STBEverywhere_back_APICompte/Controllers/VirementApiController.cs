using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.DTO;
using Microsoft.AspNetCore.Http;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APICompte.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/VirementApi")]
    [ApiController]
    public class VirementApiController : ControllerBase
    {

        private readonly ICompteRepository _dbCompte;
        private readonly IVirementRepository _dbVirement;

        private readonly ILogger<VirementApiController> _logger;
        private readonly IMapper _mapper;

        public VirementApiController(ICompteRepository dbCompte, IVirementRepository dbVirement, ILogger<VirementApiController> logger, IMapper mapper)
        {
            _dbCompte = dbCompte;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("Virement")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Virement([FromBody] VirementDto virementDto)
        {
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
}
