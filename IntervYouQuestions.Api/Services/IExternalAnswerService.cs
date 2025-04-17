namespace IntervYouQuestions.Api.Services;

public interface IExternalAnswerService
{
    Task<ExternalAnswerResponse> PostQuestion(ExternalAnswerRequest request);
}
