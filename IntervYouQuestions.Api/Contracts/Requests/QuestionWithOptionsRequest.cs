namespace IntervYouQuestions.Api.Contracts.Requests;
public record QuestionWithOptionsRequest(
    string Type,
    string Text,
    string Difficulty,
    int TopicId,
    IEnumerable<QuestionOptionRequest> Options
);

