namespace IntervYouQuestions.Api.Entities
{
    public class UserProfile
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? GitHubProfile { get; set; }
        public string? Biography { get; set; }
        public int TotalInterviewsTaken { get; set; }
        public int TotalInterviewsCompleted { get; set; }
        public double AverageScore { get; set; }
    }
}
