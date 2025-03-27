using STBEverywhere_Back_SharedModels.Models.DTO;

namespace STBEverywheres_Back_ApiAuth.Services
{
    public interface IAuthService

    {
        Task<AuthResult> Authenticate(string email, string password);
        Task<AuthResult> RefreshToken(string refreshToken);
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<string> ForgotPasswordAsync(string email);
    }
   
}
