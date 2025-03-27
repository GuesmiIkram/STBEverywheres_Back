namespace STBEverywheres_Back_ApiAuth.Extensions
{
    public static class CookieExtensions
    {
        public static void SetAuthCookies(this HttpResponse response,
      string accessToken,
      string refreshToken,
      string accessExpiry,
      string refreshExpiry)
    {
        // Safely parse expiry values with defaults
        if (!double.TryParse(accessExpiry, out double accessExpiryMinutes))
        {
            accessExpiryMinutes = 30;
        }

        if (!double.TryParse(refreshExpiry, out double refreshExpiryDays))
        {
            refreshExpiryDays = 7;
        }

        response.Cookies.Append("AccessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(accessExpiryMinutes),
            Path = "/"
        });

        response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(refreshExpiryDays),
            Path = "/api/auth/refresh-token"
        });
    }

    public static void ClearAuthCookies(this HttpResponse response)
        {
            response.Cookies.Delete("AccessToken");
            response.Cookies.Delete("RefreshToken");
        }
    }
}
