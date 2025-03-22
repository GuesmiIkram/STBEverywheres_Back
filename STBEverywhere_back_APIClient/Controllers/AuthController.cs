using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APIClient.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_ApiGateway.Service;

namespace STBEverywhere_back_APIClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
            _configuration = configuration;
        }

        // Endpoint pour l'authentification
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Tentative de connexion pour l'email : {Email}", loginDto.Email);

                // Authentifier l'utilisateur
                var client = await _authService.Authenticate(loginDto.Email, loginDto.Password);
                if (client == null)
                {
                    return Unauthorized(new { message = "Email ou mot de passe incorrect." });
                }

                // Générer le token JWT
                var token = GenerateJwtToken(client);

                // Retourner le token au client
                return Ok(new { Token = token });
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

        // Générer un token JWT
        private string GenerateJwtToken(Client client)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()), // ID du client
        new Claim(ClaimTypes.Name, client.NumCIN), // NumCin du client
        new Claim(ClaimTypes.Email, client.Email) // Email du client
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Endpoint pour rafraîchir le token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                _logger.LogInformation("Requête de rafraîchissement du token reçue.");

                // Rafraîchir les tokens
                var (newAccessToken, newRefreshToken) = await _authService.RefreshTokenAsync();

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

        // Endpoint pour l'enregistrement des utilisateurs
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Tentative d'enregistrement pour l'email : {Email}", registerDto.Email);

                // Enregistrer l'utilisateur
                var result = await _authService.RegisterAsync(registerDto);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de l'enregistrement pour l'email : {Email}. Erreur : {Message}", registerDto.Email, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        // Endpoint pour la réinitialisation du mot de passe (étape 1 : demande de réinitialisation)
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                _logger.LogInformation("Demande de réinitialisation du mot de passe pour l'email : {Email}", forgotPasswordDto.Email);

                // Générer un token de réinitialisation
                var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de la demande de réinitialisation du mot de passe pour l'email : {Email}. Erreur : {Message}", forgotPasswordDto.Email, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        // Endpoint pour réinitialiser le mot de passe (étape 2 : réinitialisation)
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                _logger.LogInformation("Tentative de réinitialisation du mot de passe avec le token : {Token}", resetPasswordDto.Token);

                // Réinitialiser le mot de passe
                var result = await _authService.ResetPasswordAsync(resetPasswordDto);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de la réinitialisation du mot de passe avec le token : {Token}. Erreur : {Message}", resetPasswordDto.Token, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        // Endpoint pour changer le mot de passe
        [HttpPost("change-password")]
        [Authorize]
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

                // Changer le mot de passe
                var result = await _authService.ChangePasswordAsync(clientId.Value, changePasswordDto);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors du changement de mot de passe. Erreur : {Message}", ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        // Endpoint pour récupérer les tokens depuis les cookies
        [HttpGet("tokens")]
        public IActionResult GetTokens()
        {
            var accessToken = Request.Cookies["AccessToken"];
            var refreshToken = Request.Cookies["RefreshToken"];

            _logger.LogInformation("Vérification des cookies dans la requête:");
            _logger.LogInformation($"AccessToken: {(string.IsNullOrEmpty(accessToken) ? "Non trouvé" : accessToken)}");
            _logger.LogInformation($"RefreshToken: {(string.IsNullOrEmpty(refreshToken) ? "Non trouvé" : refreshToken)}");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Aucun token trouvé dans les cookies.");
                return Unauthorized(new { message = "Aucun token trouvé." });
            }

            _logger.LogInformation("Tokens trouvés, envoi au client.");
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
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