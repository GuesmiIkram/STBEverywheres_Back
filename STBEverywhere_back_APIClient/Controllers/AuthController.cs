using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APIClient.Services;
using System;

namespace STBEverywhere_back_APIClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Mauvais identifiants
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Tentative de connexion pour l'email : {Email}", loginDto.Email);

                // Authentifier l'utilisateur et stocker les tokens dans les cookies
                var result = _authService.Authenticate(loginDto.Email, loginDto.Password);

                // Retourner une réponse réussie
                return Ok(new { Message = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Échec de la connexion pour l'email : {Email}. Raison : {Message}", loginDto.Email, ex.Message);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de la connexion pour l'email : {Email}. Erreur : {Message}", loginDto.Email, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Token invalide ou expiré
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur

        public IActionResult RefreshToken()
        {
            try
            {
                _logger.LogInformation("Requête de rafraîchissement du token reçue.");

                // Rafraîchir les tokens
                var (newAccessToken, newRefreshToken) = _authService.RefreshToken();
                Response.Cookies.Delete("AccessToken");
                Response.Cookies.Delete("RefreshToken");
                _authService.SetTokenCookies(newAccessToken, newRefreshToken);
                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Échec du rafraîchissement du token. Raison : {Message}", ex.Message);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors du rafraîchissement du token. Erreur : {Message}", ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        [HttpGet("tokens")]
        public IActionResult GetTokens()
        {
            var accessToken = Request.Cookies["AccessToken"];
            var refreshToken = Request.Cookies["RefreshToken"];

            Console.WriteLine(" Vérification des cookies dans la requête:");
            Console.WriteLine($" AccessToken: {(string.IsNullOrEmpty(accessToken) ? "Non trouvé" : accessToken)}");
            Console.WriteLine($" RefreshToken: {(string.IsNullOrEmpty(refreshToken) ? "Non trouvé" : refreshToken)}");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                Console.WriteLine(" Aucun token trouvé dans les cookies.");
                return Unauthorized(new { message = "Aucun token trouvé." });
            }

            Console.WriteLine("Tokens trouvés, envoi au client.");
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

    }
}
