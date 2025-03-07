
using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APIClient.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Authentifie un utilisateur avec son email et son mot de passe.
        /// </summary>
        /// <param name="email">L'email de l'utilisateur.</param>
        /// <param name="motDePasse">Le mot de passe de l'utilisateur.</param>
        /// <returns>Un message indiquant le succès de l'authentification.</returns>
        string Authenticate(string email, string motDePasse);

        /// <summary>
        /// Génère un token JWT pour un client.
        /// </summary>
        /// <param name="client">Le client pour lequel générer le token.</param>
        /// <returns>Le token JWT généré.</returns>
        string GenerateJwtToken(Client client);

        /// <summary>
        /// Rafraîchit un token JWT expiré en utilisant un refresh token.
        /// </summary>
        /// <returns>Un nouveau token JWT.</returns>
        (string AccessToken, string RefreshToken) RefreshToken();
        void SetTokenCookies(string accessToken, string refreshToken);

    }
}