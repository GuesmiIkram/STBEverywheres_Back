using Microsoft.IdentityModel.Tokens;
using STBEverywhere_back_APIClient.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using STBEverywhere_Back_SharedModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Threading.Tasks;
using BCrypt.Net;

namespace STBEverywhere_back_APIClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IClientRepository clientRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientRepository = clientRepository;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Client> Authenticate(string email, string password)
        {
            _logger.LogInformation("Tentative d'authentification pour {Email}", email);

            // Récupérer le client par email
            var client = await Task.Run(() => _clientRepository.GetClientByEmail(email));

            // Vérifier si le client existe et si le mot de passe est correct
            if (client == null || !BCrypt.Net.BCrypt.Verify(password, client.MotDePasse))
            {
                _logger.LogWarning("Mot de passe incorrect pour {Email}", email);
                throw new UnauthorizedAccessException("Mot de passe incorrect.");
            }

            // Générer les tokens JWT
            var accessToken = GenerateJwtToken(client);
            var refreshToken = GenerateRefreshToken(client);

            // Stocker les tokens dans les cookies
            SetTokenCookies(accessToken, refreshToken);

            // Retourner l'objet Client
            return client;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Tentative d'enregistrement pour l'email : {Email}", registerDto.Email);

            var existingClient = await Task.Run(() => _clientRepository.GetClientByEmail(registerDto.Email));
            if (existingClient != null)
            {
                throw new InvalidOperationException("Un client avec cet email existe déjà.");
            }

            var newClient = new Client
            {
                Email = registerDto.Email,
                MotDePasse = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            await _clientRepository.AddClientAsync(newClient);

            return "Enregistrement réussi.";
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync()
        {
            _logger.LogInformation("Tentative de rafraîchissement du token.");

            var oldRefreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(oldRefreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token manquant.");
            }

            var userId = GetUserIdFromRefreshToken(oldRefreshToken);
            var client = await Task.Run(() => _clientRepository.GetClientById(int.Parse(userId)));

            if (client == null)
            {
                throw new UnauthorizedAccessException("Refresh token invalide.");
            }

            var newAccessToken = GenerateJwtToken(client);
            var newRefreshToken = GenerateRefreshToken(client);

            SetTokenCookies(newAccessToken, newRefreshToken);

            return (newAccessToken, newRefreshToken);
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Demande de réinitialisation du mot de passe pour l'email : {Email}", email);

            var client = await Task.Run(() => _clientRepository.GetClientByEmail(email));
            if (client == null)
            {
                throw new InvalidOperationException("Aucun client trouvé avec cet email.");
            }

            var resetToken = Guid.NewGuid().ToString();
            client.ResetPasswordToken = resetToken;
            client.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _clientRepository.UpdateClientAsync(client);

            return "Un e-mail de réinitialisation a été envoyé.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Tentative de réinitialisation du mot de passe avec le token : {Token}", resetPasswordDto.Token);

            var client = await Task.Run(() => _clientRepository.GetClientByResetToken(resetPasswordDto.Token));
            if (client == null || client.ResetPasswordTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Token invalide ou expiré.");
            }

            client.MotDePasse = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            client.ResetPasswordToken = null;
            client.ResetPasswordTokenExpiry = null;

            await _clientRepository.UpdateClientAsync(client);

            return "Mot de passe réinitialisé avec succès.";
        }

        public async Task<string> ChangePasswordAsync(int clientId, ChangePasswordDto changePasswordDto)
        {
            _logger.LogInformation("Tentative de changement de mot de passe pour l'utilisateur avec l'ID : {ClientId}", clientId);

            var client = await Task.Run(() => _clientRepository.GetClientById(clientId));
            if (client == null)
            {
                throw new InvalidOperationException("Client non trouvé.");
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, client.MotDePasse))
            {
                throw new UnauthorizedAccessException("L'ancien mot de passe est incorrect.");
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                throw new InvalidOperationException("Les nouveaux mots de passe ne correspondent pas.");
            }

            client.MotDePasse = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _clientRepository.UpdateClientAsync(client);

            return "Mot de passe changé avec succès.";
        }

        public string GenerateJwtToken(Client client)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("Clé JWT non configurée.");
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                    new Claim(ClaimTypes.Email, client.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken(Client client)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("Clé JWT non configurée.");
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", client.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void SetTokenCookies(string accessToken, string refreshToken)
        {
            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(120),
                Path = "/"
            };

            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, accessTokenCookieOptions);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, refreshTokenCookieOptions);
        }

        private string GetUserIdFromRefreshToken(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(refreshToken);

                var userId = token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Refresh token invalide : ID manquant.");
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError("Erreur lors de la lecture du refresh token : {Message}", ex.Message);
                throw new UnauthorizedAccessException("Refresh token invalide.");
            }
        }

        public (string AccessToken, string RefreshToken) RefreshToken()
        {
            _logger.LogInformation("Tentative de rafraîchissement du token.");

            var oldRefreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(oldRefreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token manquant.");
            }

            var userId = GetUserIdFromRefreshToken(oldRefreshToken);
            var client = _clientRepository.GetClientById(int.Parse(userId));

            if (client == null)
            {
                throw new UnauthorizedAccessException("Refresh token invalide.");
            }

            var newAccessToken = GenerateJwtToken(client);
            var newRefreshToken = GenerateRefreshToken(client);

            SetTokenCookies(newAccessToken, newRefreshToken);

            return (newAccessToken, newRefreshToken);
        }
    }
}