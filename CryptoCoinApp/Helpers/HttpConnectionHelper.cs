using CryptoCoinApp.Helpers.Interface;
using RestSharp;
using System.Text;
using System.Text.Json;
namespace CryptoCoinApp.Helpers;

public class HttpConnectionHelper : IHttpConnectionHelper
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
                result = JsonSerializer.Deserialize<T>(response.Content);
                var codeProperty = typeof(T).GetProperty("Code");
                if (codeProperty != null) codeProperty.SetValue(result, "00");
                var descriptionProperty = typeof(T).GetProperty("Description");
                if (descriptionProperty != null) descriptionProperty.SetValue(result, "Successful");
            }
            else
            {
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
