namespace IntervYouQuestions.Api.Contracts.Responses;

public record ModelAnswerResponse(
    int ModelAnswerId,
    string Text,
    string KeyPoints
);
