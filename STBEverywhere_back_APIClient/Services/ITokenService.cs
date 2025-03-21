namespace STBEverywhere_ApiGateway.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(string email);
        string GenerateRefreshToken();
    }
}
