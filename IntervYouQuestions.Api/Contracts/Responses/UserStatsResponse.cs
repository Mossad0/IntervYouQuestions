namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class UserStatsResponse
    {
        public string UserId { get; set; }
        public int TotalInterviews { get; set; }
        public int CompletedInterviews { get; set; }
        public double AverageScore { get; set; }
        public List<InterviewSummary> RecentInterviews { get; set; } = new List<InterviewSummary>();
        public Dictionary<string, double> TopicPerformance { get; set; } = new Dictionary<string, double>();
    }
    public class InterviewSummary
    {
        public string InterviewId { get; set; }
        public string Role { get; set; }
        public string ExperienceLevel { get; set; }
        public string Date { get; set; }
        public double Score { get; set; }
        public int QuestionsAnswered { get; set; }
        public int TotalQuestions { get; set; }
    }
}
