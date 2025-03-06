namespace IntervYouQuestions.Api.Contracts.Requests;

public record QuestionOptionRequest(
    string Text,
    bool IsCorrect
);
