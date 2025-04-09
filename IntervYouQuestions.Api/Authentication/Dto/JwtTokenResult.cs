namespace IntervYouQuestions.Api.Authentication.Dto;

public class JwtTokenResult
{
    public string Token { get; set; }
    public string Jti { get; set; }
    public DateTime ExpiresAt { get; set; }
}

