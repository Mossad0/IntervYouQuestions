namespace IntervYouQuestions.Api.Authentication;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; } // From JWT's Jti
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; } = false;
    public bool IsRevoked { get; set; } = false;

    public AppUser User { get; set; }
}

