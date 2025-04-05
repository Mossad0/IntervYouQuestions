namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
       
        public string ExperienceLevel { get; set; }
        public string Role { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
