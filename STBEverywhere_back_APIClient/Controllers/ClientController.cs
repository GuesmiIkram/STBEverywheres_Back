using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APIClient.Services;

namespace STBEverywhere_back_APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

      
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> GetClientInfo()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var client = await _clientService.GetClientByIdAsync(clientId.Value);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            return Ok(client);
        }

        // Mettre à jour les informations du client
        [HttpPut("update")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Token invalide ou expiré
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Client non trouvé
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public async Task<IActionResult> UpdateClientInfo([FromBody] Client updatedClient)
        {
            // Récupérer l'ID du client à partir du token JWT
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            // Appeler le service pour mettre à jour les informations du client
            bool isUpdated = await _clientService.UpdateClientInfoAsync(clientId.Value, updatedClient);
            if (!isUpdated)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            return Ok(new { message = "Informations mises à jour avec succès !" });
        }

        // Extraire l'ID du client à partir du Token JWT
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