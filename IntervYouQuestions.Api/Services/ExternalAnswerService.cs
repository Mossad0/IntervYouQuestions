
using System.Net.Http;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IntervYouQuestions.Api.Services;

public class ExternalAnswerService : IExternalAnswerService
{
    private readonly HttpClient _httpClient; // Injected directly
    private readonly ILogger<ExternalAnswerService> _logger;

    // HttpClient is injected by the factory
    public ExternalAnswerService(HttpClient httpClient, ILogger<ExternalAnswerService> logger)
    {
        _httpClient = httpClient; // No need for IHttpClientFactory here
        _logger = logger;
    }



public async Task<ExternalAnswerResponse> PostQuestion(ExternalAnswerRequest request)
{
    string requestUrl = "essay_similer";

    // *** LOG THE REQUEST ***
    string jsonPayload = "Serialization Error";
    try
    {
        // Use appropriate options if needed (e.g., camelCase)
        var options = new JsonSerializerOptions { WriteIndented = true /*, PropertyNamingPolicy = JsonNamingPolicy.CamelCase */ };
        jsonPayload = JsonSerializer.Serialize(request, options);
        _logger.LogInformation("Sending request to {BaseAddress}{RequestUrl} with payload:\n{Payload}",
            _httpClient.BaseAddress, requestUrl, jsonPayload);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error serializing request payload.");
        // Handle this failure appropriately
    }
    // *** END LOGGING ***

    try // Wrap the API call
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUrl, request);

        // Check status code BEFORE EnsureSuccessStatusCode to log the error response
        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("External API returned Status {StatusCode}. Response Body: {ErrorBody}",
                (int)response.StatusCode, errorContent);
            // Now it will throw when EnsureSuccessStatusCode is called
        }

        response.EnsureSuccessStatusCode(); // Throws HttpRequestException on non-2xx

        var responseData = await response.Content.ReadFromJsonAsync<ExternalAnswerResponse>();
        return responseData!;
    }
    catch (HttpRequestException httpEx)
    {
        // Logging of error body happened above if status code was bad
        _logger.LogError(httpEx, "HTTP error calling external API {Url}. Status Code was {StatusCode}.",
            _httpClient.BaseAddress + requestUrl, httpEx.StatusCode); // BaseAddress might be null
                                                                      // Rethrow, return a default/error response, or handle as appropriate for your API
        throw;
    }
    catch (JsonException jsonEx)
    {
        _logger.LogError(jsonEx, "Error deserializing successful response from external API {Url}.", _httpClient.BaseAddress + requestUrl);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred calling external API {Url}.", _httpClient.BaseAddress + requestUrl);
        throw;
    }
}
}
