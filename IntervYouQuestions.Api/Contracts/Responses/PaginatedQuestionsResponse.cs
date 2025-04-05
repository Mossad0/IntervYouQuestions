namespace IntervYouQuestions.Api.Contracts.Responses
{
    public class PaginatedQuestionsResponse
    {
        public int TotalQuestions { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<QuestionListItemResponse> Questions { get; set; } = new();
    }
}
