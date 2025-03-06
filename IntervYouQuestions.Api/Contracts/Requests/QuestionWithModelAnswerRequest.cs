namespace IntervYouQuestions.Api.Contracts.Requests;

public record QuestionWithModelAnswerRequest(
    string Type,
    string Text,
    string Difficulty,
    int TopicId,
    IEnumerable<ModelAnswerRequest> ModelAnswers
);

