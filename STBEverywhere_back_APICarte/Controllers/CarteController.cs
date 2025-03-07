using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_back_APICarte.Services;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Security.Claims;

namespace STBEverywhere_back_APICarte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarteController : ControllerBase
    {
        private readonly ICarteService _carteService;

        public CarteController(ICarteService carteService)
        {
            _carteService = carteService;
        }

        [HttpGet("rib/{rib}")]

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CarteDTO>>> GetCartesByRIB(string rib)
        {

            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            else {
                var cartes = await _carteService.GetCartesByRIBAsync(rib);
                return Ok(cartes);

            }
            
        }

        [HttpPost("demande")]

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateDemandeCarte(DemandeCarteDTO demandeCarteDTO)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
            else
            {
                try
                {
                    var result = await _carteService.CreateDemandeCarteAsync(demandeCarteDTO);
                    if (result)
                    {
                        return Ok("Demande de carte créée avec succès.");
                    }
                    return BadRequest("Erreur lors de la création de la demande de carte.");
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("demandes/rib/{rib}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DemandeCarteDTO>>> GetDemandesByRIB(string rib)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var demandes = await _carteService.GetDemandesByRIBAsync(rib);
            return Ok(demandes);
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

        [HttpGet("details/{numCarte}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CarteDTO>> GetCarteDetails(string numCarte)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            try
            {
                var carteDetails = await _carteService.GetCarteDetailsAsync(numCarte);
                return Ok(carteDetails);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
