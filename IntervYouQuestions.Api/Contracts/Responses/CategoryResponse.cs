namespace IntervYouQuestions.Api.Contracts.Responses;

public record CategoryResponse
(
    int CategoryId, 
    string Name,
    decimal Weight,
    IEnumerable<TopicResponse> Topics
);
