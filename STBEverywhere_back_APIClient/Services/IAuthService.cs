using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Threading.Tasks;

namespace STBEverywhere_back_APIClient.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Authentifie un utilisateur avec son email et son mot de passe.
        /// </summary>
         Task<Client> Authenticate(string email, string password);

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        Task<string> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Rafraîchit un token JWT expiré en utilisant un refresh token.
        /// </summary>
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync();

        /// <summary>
        /// Envoie un e-mail de réinitialisation de mot de passe.
        /// </summary>
        Task<string> ForgotPasswordAsync(string email);

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur.
        /// </summary>
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        /// <summary>
        /// Permet à un utilisateur de changer son mot de passe.
        /// </summary>
        Task<string> ChangePasswordAsync(int clientId, ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Génère un token JWT pour un client.
        /// </summary>
        string GenerateJwtToken(Client client);

        /// <summary>
        /// Rafraîchit un token JWT expiré en utilisant un refresh token.
        /// </summary>
        (string AccessToken, string RefreshToken) RefreshToken();

        /// <summary>
        /// Stocke les tokens dans les cookies.
        /// </summary>
        void SetTokenCookies(string accessToken, string refreshToken);
    }
}