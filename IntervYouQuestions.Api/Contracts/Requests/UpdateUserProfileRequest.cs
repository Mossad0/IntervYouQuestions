namespace IntervYouQuestions.Api.Contracts.Requests
{
    public class UpdateUserProfileRequest
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string LinkedInProfile { get; set; }
        public string GitHubProfile { get; set; }
        public string Biography { get; set; }
    }
}
