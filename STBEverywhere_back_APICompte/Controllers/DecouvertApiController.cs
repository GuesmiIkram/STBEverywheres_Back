using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_ApiAuth.Repositories;
using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_back_APICompte.Services;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace STBEverywhere_back_APICompte.Controllers
{

    [Route("api/Decouvert")]
    [ApiController]
    public class DecouvertApiController:ControllerBase
    {

        private readonly ICompteService _compteService;
      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DecouvertApiController> _logger;
  
        public DecouvertApiController(ICompteService compteService, IHttpContextAccessor httpContextAccessor, ILogger<DecouvertApiController> logger)
        {
            _compteService = compteService;
           
            _httpContextAccessor = httpContextAccessor;
        }




        [HttpGet("getDemandesByClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDemandesByClient()
        {
            try
            {
                // Récupérer l'ID du client à partir du token
                var clientId = GetUserIdFromToken();

                // Récupérer les demandes de modification de découvert pour ce client
                var demandes = await _compteService.GetDemandesByClientIdAsync(clientId);

                if (!demandes.Any())
                {
                    return NotFound(new { message = "Aucune demande trouvée pour ce client." });
                }

                return Ok(demandes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpPost("demandeModificationDecouvert")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DemandeModificationDecouvert([FromBody] DemandeModificationDecouvertDto demandeDto)
        {
            if (demandeDto == null || string.IsNullOrEmpty(demandeDto.RIBCompte))
            {
                return BadRequest(new { message = "Le RIB et le montant du découvert demandé sont obligatoires." });
            }

            // Vérifier si le compte existe
            var compte = await _compteService.GetByRIBAsync(demandeDto.RIBCompte);
            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable." });
            }

            // Vérifier si une demande pour ce compte est déjà en attente
            var existingDemandes = await _compteService.GetDemandesModificationAsync(demandeDto.RIBCompte, "En attente");
            if (existingDemandes.Any())
            {
                return BadRequest(new { message = "Une demande de modification de découvert est déjà en attente pour ce compte." });
            }

            // Créer une nouvelle demande
            var demande = new DemandeModificationDecouvert
            {
                RIBCompte = demandeDto.RIBCompte,
                DecouvertDemande = demandeDto.DecouvertDemande,
                StatutDemande = "En attente",
                DateDemande = DateTime.Now
            };

            await _compteService.CreateDemandeModificationAsync(demande);

            return CreatedAtAction(nameof(DemandeModificationDecouvert), new { id = demande.Id }, demande);
        }




        [HttpGet("getDecouvertAutorise/{rib}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDecouvertAutorise(string rib)
        {
            if (string.IsNullOrEmpty(rib))
            {
                return BadRequest(new { message = "Le RIB est obligatoire." });
            }

            // Vérifier si le compte existe
            var compte = await _compteService.GetByRIBAsync(rib);
            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable." });
            }

            return Ok(new { DecouvertAutorise = compte.DecouvertAutorise });
        }


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


    }
}
