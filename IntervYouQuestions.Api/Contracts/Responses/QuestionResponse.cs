namespace IntervYouQuestions.Api.Contracts.Responses;

public record QuestionResponse(
    int QuestionId,
    string Type,
    string Text,
    string Difficulty,
    int TopicId,
    IEnumerable<QuestionOptionResponse>? QuestionOptions,
    IEnumerable<ModelAnswerResponse>? ModelAnswers

    /// This is comment
);
