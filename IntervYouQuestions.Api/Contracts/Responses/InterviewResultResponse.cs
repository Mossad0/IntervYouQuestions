namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class InterviewResultResponse
    {
        public int InterviewId { get; }
        public string Title { get; }
        public string ExperienceLevel { get; }
        public int TotalQuestions { get; }
        public int AnsweredQuestions { get; }
        public int CorrectAnswers { get; }
        public double ScorePercentage { get; }
        public List<QuestionResultResponse> DetailedResults { get; }

        public InterviewResultResponse(
            int interviewId,
            string title,
            string experienceLevel,
            int totalQuestions,
            int answeredQuestions,
            int correctAnswers,
            double scorePercentage,
            List<QuestionResultResponse> detailedResults)
        {
            InterviewId = interviewId;
            Title = title;
            ExperienceLevel = experienceLevel;
            TotalQuestions = totalQuestions;
            AnsweredQuestions = answeredQuestions;
            CorrectAnswers = correctAnswers;
            ScorePercentage = scorePercentage;
            DetailedResults = detailedResults;
        }
    }
}
