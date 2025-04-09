namespace IntervYouQuestions.Api.Authentication;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int TokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInDays { get; set; }

}

