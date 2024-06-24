using CryptoCoinApp.Helpers.Interface;
using RestSharp;
using Serilog;
using System.Text;
using System.Text.Json;

namespace CryptoCoinApp.Helpers
{
    /*NB: Please note tha a specific type of object can be used as response
    {
      code = string,
      description = string,
      data = object
    }
    When code is 27, the response was unsuccessful
    When code is 77, this logic broke somewhere
    */
    public class HttpConnectionHelper : IHttpConnectionHelper
    {
        private readonly ILogger<HttpConnectionHelper> _logger;

        public HttpConnectionHelper(ILogger<HttpConnectionHelper> logger)
        {
            _logger = logger;
        }

        public async Task<T> AppRequest<T>(
            string url,
            string requestType,
            object? requestBody = null,
            Dictionary<string, string>? headers = null,
            string? user = null,
            string? password = null) where T : new()
        {
            T result = new();
            try
            {
                _logger.LogInformation("Starting request to {Url} with request type {RequestType}", url, requestType);

                var client = new RestClient(url);
                var request = new RestRequest(url, requestType.ToLower() == "post" ? Method.Post : Method.Get);

                if (requestType.ToLower() == "post" && requestBody != null)
                {
                    request.AddJsonBody(requestBody);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{user}:{password}");
                    var value = Convert.ToBase64String(byteArray);
                    request.AddHeader("Authorization", $"Basic {value}");
                }

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Request to {Url} was successful", url);
                    result = JsonSerializer.Deserialize<T>(response.Content);
                    var codeProperty = typeof(T).GetProperty("Code");
                    if (codeProperty != null) codeProperty.SetValue(result, "00");
                    var descriptionProperty = typeof(T).GetProperty("Description");
                    if (descriptionProperty != null) descriptionProperty.SetValue(result, "Successful");
                }
                else
                {
                    _logger.LogError("Request to {Url} failed with status code {StatusCode} and response {Response}", url, response.StatusCode, response.Content);
                    var errorResponse = new
                    {
                        code = "24",
                        description = response.Content
                    };
                    result = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(errorResponse));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during request to {Url}", url);
                var errorResponse = new
                {
                    code = "77",
                    description = ex.Message
                };
                result = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(errorResponse));
            }
            return result;
        }
    }
}
