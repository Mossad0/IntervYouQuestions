namespace IntervYouQuestions.Api.Contracts.Responses;

public record QuestionOptionResponse(
    int OptionId,
    string Text,
    bool IsCorrect
);
