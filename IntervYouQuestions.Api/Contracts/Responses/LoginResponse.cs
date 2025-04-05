namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class LoginResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ExperienceLevel { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
