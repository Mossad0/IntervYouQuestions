using IntervYouQuestions.Api.Entities;

namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class InterviewResponse
    {
        public string InterviewId { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public RoleType Role { get; set; }
        public int TimeLimitInMinutes { get; set; }
        public List<int> QuestionIds { get; set; }
        public QuestionResponse CurrentQuestion { get; set; }
    }
}
