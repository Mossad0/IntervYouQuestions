namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class UserProfileResponse
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string LinkedInProfile { get; set; }
        public string GitHubProfile { get; set; }
        public string Biography { get; set; }
        public string ExperienceLevel { get; set; }
        public string Role { get; set; }
        public int TotalInterviewsTaken { get; set; }
        public int TotalInterviewsCompleted { get; set; }
        public double AverageScore { get; set; }
    }
}
