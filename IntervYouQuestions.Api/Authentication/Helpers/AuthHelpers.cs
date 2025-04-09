using System.Security.Cryptography;

namespace IntervYouQuestions.Api.Authentication.Helpers;

public static class AuthHelpers
{
    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

}
