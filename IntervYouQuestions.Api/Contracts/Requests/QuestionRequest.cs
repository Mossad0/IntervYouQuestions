namespace IntervYouQuestions.Api.Contracts.Requests;

public record QuestionRequest(
    string Type,
    string Text,
    string Difficulty,
    int TopicId,
    IEnumerable<QuestionOptionRequest> Options
);
