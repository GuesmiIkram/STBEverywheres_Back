using Microsoft.IdentityModel.Tokens;

using STBEverywhere_back_APIClient.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using STBEverywhere_Back_SharedModels;
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

        public string Authenticate(string email, string motDePasse)
        {
            _logger.LogInformation("Tentative d'authentification pour {Email}", email);
            var client = _clientRepository.GetClientByEmail(email);

            if (client == null || !BCrypt.Net.BCrypt.Verify(motDePasse, client.MotDePasse))
            {
                _logger.LogWarning("Mot de passe incorrect pour {Email}", email);
                throw new UnauthorizedAccessException("Mot de passe incorrect.");
            }

            // Générer un access token et un refresh token
            var accessToken = GenerateJwtToken(client);
            var refreshToken = GenerateRefreshToken(client);

            // Stocker les tokens dans les cookies
            SetTokenCookies(accessToken, refreshToken);

            return "Authentification réussie.";
        }

        public (string AccessToken, string RefreshToken) RefreshToken()
        {
            _logger.LogInformation("Tentative de rafraîchissement du token.");

            // Récupérer le refresh token depuis les cookies
            var oldRefreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(oldRefreshToken))
            {
                _logger.LogWarning("Refresh token manquant dans les cookies.");
                throw new UnauthorizedAccessException("Refresh token manquant.");
            }

            _logger.LogInformation("Refresh token récupéré depuis les cookies : {RefreshToken}", oldRefreshToken);

            // Récupérer l'ID de l'utilisateur depuis le refresh token
            var userId = GetUserIdFromRefreshToken(oldRefreshToken);
            _logger.LogInformation("ID extrait du refresh token : {UserId}", userId);

            // Récupérer l'utilisateur associé à l'ID
            var client = _clientRepository.GetClientById(int.Parse(userId));

            if (client == null)
            {
                _logger.LogWarning("Aucun utilisateur trouvé pour l'ID : {UserId}", userId);
                throw new UnauthorizedAccessException("Refresh token invalide.");
            }

            // Générer un nouvel access token et un nouveau refresh token
            var newAccessToken = GenerateJwtToken(client);
            var newRefreshToken = GenerateRefreshToken(client);

            _logger.LogInformation("Nouvel access token et refresh token générés.");

            // Mettre à jour les cookies avec les nouveaux tokens
            SetTokenCookies(newAccessToken, newRefreshToken);

            // Retourner les deux tokens
            return (newAccessToken, newRefreshToken);
        }


        public string GenerateJwtToken(Client client)
        {
            _logger.LogInformation("Génération du token JWT pour l'utilisateur avec l'ID : {ClientId}", client.Id);
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError("Clé JWT non définie dans la configuration.");
                throw new Exception("Erreur interne : clé JWT non configurée.");
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
                Expires = DateTime.UtcNow.AddMinutes(15), // Durée de vie courte pour l'access token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken(Client client)
        {
            _logger.LogInformation("Génération du refresh token JWT pour l'utilisateur avec l'ID : {ClientId}", client.Id);
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError("Clé JWT non définie dans la configuration.");
                throw new Exception("Erreur interne : clé JWT non configurée.");
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", client.Id.ToString()) // Utiliser un nom de claim explicite
                }),
                Expires = DateTime.UtcNow.AddDays(1), // Durée de vie plus longue pour le refresh token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public  void SetTokenCookies(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false, // Angular peut accéder au cookie
                Secure = false,// ⚠️ `false` en localhost, `true` en prod
                SameSite = SameSiteMode.Lax, // 🌟 Permet d'envoyer les cookies avec requêtes cross-origin
                Expires = DateTime.UtcNow.AddHours(1),
                Path = "/"
            };

            // Ajouter des logs pour vérifier les cookies avant de les définir
            _logger.LogInformation("Tentative de définition des cookies :");
            _logger.LogInformation("AccessToken : {AccessToken}", accessToken);
            _logger.LogInformation("RefreshToken : {RefreshToken}", refreshToken);
            _logger.LogInformation("Options des cookies : HttpOnly={HttpOnly}, Expires={Expires}, SameSite={SameSite}, Secure={Secure}, Path={Path}",
                cookieOptions.HttpOnly, cookieOptions.Expires, cookieOptions.SameSite, cookieOptions.Secure, cookieOptions.Path);

            // Définir les cookies dans la réponse HTTP
            _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, cookieOptions);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            // Ajouter un log pour confirmer que les cookies ont été définis
            _logger.LogInformation("Cookies définis avec succès : AccessToken et RefreshToken");
        }
        /*
        private void SetAccessTokenCookie(string accessToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddHours(1), // Durée de vie courte pour l'access token
                SameSite = SameSiteMode.Lax, // SameSite=Lax en développement
                Secure = false,// Secure=false en développement (HTTP)
                Path = "/", // Définit le chemin du cookie
            };

            // Ajouter des logs pour vérifier le cookie avant de le définir
            _logger.LogInformation("Tentative de définition du cookie AccessToken :");
            _logger.LogInformation("AccessToken : {AccessToken}", accessToken);
            _logger.LogInformation("Options du cookie : HttpOnly={HttpOnly}, Expires={Expires}, SameSite={SameSite}, Secure={Secure}, Path={Path}",
                cookieOptions.HttpOnly, cookieOptions.Expires, cookieOptions.SameSite, cookieOptions.Secure, cookieOptions.Path);

            // Définir le cookie dans la réponse HTTP
            _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, cookieOptions);

            // Ajouter un log pour confirmer que le cookie a été défini
            _logger.LogInformation("Cookie AccessToken défini avec succès.");
        } */
        private string GetUserIdFromRefreshToken(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(refreshToken);

                // Log tous les claims pour déboguer
                foreach (var claim in token.Claims)
                {
                    _logger.LogInformation("Claim trouvé : {Type} = {Value}", claim.Type, claim.Value);
                }

                // Récupérer l'ID de l'utilisateur depuis le refresh token
                var userId = token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value; // Utiliser le même nom de claim
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
    }
}