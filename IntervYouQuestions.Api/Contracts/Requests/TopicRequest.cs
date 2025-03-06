namespace IntervYouQuestions.Api.Contracts.Requests;

public record TopicRequest
    (
        string Name,
        int CategoryId
    );
