using Microsoft.IdentityModel.Tokens;
using STBEverywhere_ApiAuth.Repositories;
using STBEverywhere_back_APIClient.Services;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace STBEverywheres_Back_ApiAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResult> Authenticate(string email, string password)
        {
            _logger.LogInformation("Tentative d'authentification pour {Email}", email);

            var user = await _userRepository.GetUserWithClientByEmailAsync(email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Authentication failed for {Email}", email);
                return null;
            }

            return new AuthResult
            {
                AccessToken = GenerateToken(user, isAccessToken: true),
                RefreshToken = GenerateToken(user, isAccessToken: false),
                Role = user.Role,
                UserId = user.Id,
               
            };
        }

        public async Task<AuthResult> RefreshToken(string refreshToken)
        {
            try
            {
                var principal = ValidateToken(refreshToken, isAccessToken: false);
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    throw new SecurityTokenException("Invalid token claims");
                }

                var user = await _userRepository.GetByIdAsync(int.Parse(userId));
                if (user == null)
                {
                    throw new SecurityTokenException("User not found");
                }

                return new AuthResult
                {
                    AccessToken = GenerateToken(user, isAccessToken: true),
                    RefreshToken = GenerateToken(user, isAccessToken: false),
                    Role = user.Role,
                    UserId = user.Id,
                   
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token validation failed");
                throw new SecurityTokenException("Invalid refresh token", ex);
            }
        }
        private string GenerateToken(User user, bool isAccessToken)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
       // new Claim("clientid", user.Client?.Id.ToString() ?? user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                isAccessToken
                    ? _configuration["Jwt:Key"]
                    : _configuration["Jwt:RefreshKey"]))
            {
                KeyId = isAccessToken ? "access_key" : "refresh_key"
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(
                    isAccessToken
                        ? TimeSpan.FromMinutes(int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"]))
                        : TimeSpan.FromDays(int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]))),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal ValidateToken(string token, bool isAccessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = isAccessToken
                ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])) { KeyId = "access_key" }
                : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:RefreshKey"])) { KeyId = "refresh_key" };

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Password reset request for email: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException("No user found with this email.");
            }

            var resetToken = Guid.NewGuid().ToString(); // Générer un token unique
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1); // Expiration du token après 1 heure

            await _userRepository.UpdateAsync(user);

            // Créer l'URL pour la réinitialisation du mot de passe
            var resetPasswordUrl = $"http://localhost:4200/reset-password?token={resetToken}";

            // Créer le contenu de l'email
            var subject = "Réinitialisation de votre mot de passe";
            var body = $"<p>Bonjour,</p><p>Nous avons reçu une demande de réinitialisation de votre mot de passe. Veuillez cliquer sur le lien ci-dessous pour réinitialiser votre mot de passe :</p><p><a href='{resetPasswordUrl}'>Réinitialiser le mot de passe</a></p><p>Ce lien expirera dans 1 heure.</p>";

            // Appel du service d'envoi d'email
            var emailService = new EmailService(_configuration);
            await emailService.SendEmailAsync(user.Email, subject, body);

            return "Password reset instructions have been sent to your email.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Password reset attempt with token: {Token}", resetPasswordDto.Token);

            var user = await _userRepository.GetByResetTokenAsync(resetPasswordDto.Token);
            if (user == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _userRepository.UpdateAsync(user);

            return "Password has been reset successfully.";
        }
    }
}