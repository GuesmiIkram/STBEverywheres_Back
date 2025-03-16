using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APIClient.Services;
using System;
using STBEverywhere_back_APIClient.Repositories;
using STBEverywhere_Back_SharedModels;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace STBEverywhere_back_APIClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IClientRepository _clientRepository;
        private readonly HttpClient _httpClient;
        private readonly EmailService _emailService;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger,
            IClientRepository clientRepository,
            HttpClient httpClient,
            EmailService emailService)
        {
            _authService = authService;
            _logger = logger;
            _clientRepository = clientRepository;
            _httpClient = httpClient;
            _emailService = emailService;
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

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Requête invalide
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Client déjà enregistré
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Client inexistant
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Tentative d'enregistrement pour l'email : {Email}", registerDto.Email);

                // Appeler l'API du service de compte pour vérifier si le RIB existe
                var response = await _httpClient.GetAsync($"http://localhost:5185/api/CompteApi/GetByRIB/{registerDto.RIB}");
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { Message = "RIB invalide." });
                }

                // Vérifier si le client associé à ce RIB existe
                var existingClient = _clientRepository.GetClientByEmail(registerDto.Email);
                if (existingClient == null)
                {
                    return NotFound(new { Message = "Client inexistant." });
                }

                // Vérifier si le client possède déjà un mot de passe
                if (!string.IsNullOrEmpty(existingClient.MotDePasse))
                {
                    return Conflict(new { Message = "Le client est déjà enregistré." });
                }

                // Mettre à jour le client avec le nouveau mot de passe
                existingClient.MotDePasse = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                await _clientRepository.UpdateClientAsync(existingClient);

                return Ok(new { Message = "Enregistrement réussi." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de l'enregistrement pour l'email : {Email}. Erreur : {Message}", registerDto.Email, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Client inexistant
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                _logger.LogInformation("Demande de réinitialisation du mot de passe pour l'email : {Email}", forgotPasswordDto.Email);

                // Vérifier si le client existe
                var existingClient = _clientRepository.GetClientByEmail(forgotPasswordDto.Email);
                if (existingClient == null)
                {
                    return NotFound(new { Message = "Client inexistant." });
                }

                // Générer un token de réinitialisation
                var resetToken = Guid.NewGuid().ToString();
                existingClient.ResetPasswordToken = resetToken;
                existingClient.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valide pendant 1 heure

                // Mettre à jour le client dans la base de données
                await _clientRepository.UpdateClientAsync(existingClient);

                // Envoyer un e-mail avec le lien de réinitialisation
                var resetLink = $"http://localhost:5260/api/Auth/reset-password?token={resetToken}";
                await _emailService.SendEmailAsync(existingClient.Email, "Réinitialisation de votre mot de passe", $"Cliquez sur ce lien pour réinitialiser votre mot de passe : {resetLink}");

                return Ok(new { Message = "Un e-mail de réinitialisation a été envoyé." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de la demande de réinitialisation du mot de passe pour l'email : {Email}. Erreur : {Message}", forgotPasswordDto.Email, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        [HttpGet("reset-password")]
        public IActionResult ShowResetPasswordForm([FromQuery] string token)
        {
            // Vérifier si le token est valide
            var existingClient = _clientRepository.GetClientByResetToken(token);
            if (existingClient == null || existingClient.ResetPasswordTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Token invalide ou expiré." });
            }

            // Afficher une page HTML avec un formulaire de réinitialisation
            return Content(@"
                <form action='/api/Auth/reset-password' method='post'>
                    <input type='hidden' name='token' value='" + token + @"' />
                    <label for='newPassword'>Nouveau mot de passe :</label>
                    <input type='password' id='newPassword' name='newPassword' required />
                    <button type='submit'>Réinitialiser</button>
                </form>
            ", "text/html");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Token invalide ou expiré
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Client inexistant
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public async Task<IActionResult> ResetPassword([FromForm] string token, [FromForm] string newPassword)
        {
            try
            {
                _logger.LogInformation("Tentative de réinitialisation du mot de passe avec le token : {Token}", token);

                // Vérifier si le token est valide
                var existingClient = _clientRepository.GetClientByResetToken(token);
                if (existingClient == null || existingClient.ResetPasswordTokenExpiry < DateTime.UtcNow)
                {
                    return BadRequest(new { Message = "Token invalide ou expiré." });
                }

                // Mettre à jour le mot de passe
                existingClient.MotDePasse = BCrypt.Net.BCrypt.HashPassword(newPassword);
                existingClient.ResetPasswordToken = null; // Réinitialiser le token
                existingClient.ResetPasswordTokenExpiry = null; // Réinitialiser l'expiration

                // Mettre à jour le client dans la base de données
                await _clientRepository.UpdateClientAsync(existingClient);

                return Ok(new { Message = "Mot de passe réinitialisé avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de la réinitialisation du mot de passe avec le token : {Token}. Erreur : {Message}", token, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }


        [HttpPost("change-password")]
        [Authorize] // Seuls les utilisateurs authentifiés peuvent changer leur mot de passe
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Requête invalide
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Non autorisé
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Client non trouvé
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                // Récupérer l'ID du client à partir du token JWT
                var clientId = GetClientIdFromToken();
                if (clientId == null)
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                // Récupérer le client depuis la base de données
                var client = await _clientRepository.GetClientByIdAsync(clientId.Value);
                if (client == null)
                {
                    return NotFound(new { Message = "Client non trouvé." });
                }

                // Vérifier si l'ancien mot de passe est correct
                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, client.MotDePasse))
                {
                    return BadRequest(new { Message = "L'ancien mot de passe est incorrect." });
                }

                // Vérifier si le nouveau mot de passe et la confirmation correspondent
                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                {
                    return BadRequest(new { Message = "Les nouveaux mots de passe ne correspondent pas." });
                }

                // Hacher et mettre à jour le nouveau mot de passe
                client.MotDePasse = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                await _clientRepository.UpdateClientAsync(client);

                return Ok(new { Message = "Mot de passe changé avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors du changement de mot de passe. Erreur : {Message}", ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        // Méthode pour extraire l'ID du client à partir du token JWT
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