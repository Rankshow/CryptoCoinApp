using CryptoCoinApp.Helpers.Interface;
using RestSharp;
using System.Text;
using System.Text.Json;
namespace CryptoCoinApp.Helpers;

/*
public class HttpConnectionHelper : IHttpConnectionHelper
{
    public async Task<T> AppRequest<T>(
        string url,
        string requestType,
        object? requestBody = null,
        Dictionary<string, string>? headers = null,
        string? user = null,
        string? password = null
    ) where T : new()
    {
        T result = new();
        try
        {
            var client = new RestClient(url);
            Method method = requestType.ToLower() == "post" ? Method.Post : Method.Get;

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
                request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(byteArray));
            }

            var response = await client.ExecuteAsync(request);
            var responseContent = response.Content;

            if (response.IsSuccessful)
            {
                result = JsonSerializer.Deserialize<T>(responseContent);
                var codeProperty = typeof(T).GetProperty("Code");
                codeProperty?.SetValue(result, "00");
                var descriptionProperty = typeof(T).GetProperty("Description");
                descriptionProperty?.SetValue(result, "Successful");
            }
            else
            {
                var errorResponse = new
                {
                    code = "24",
                    description = responseContent
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
*/


public class HttpConnectionHelper : IHttpConnectionHelper
{
    public async Task<T> AppRequest<T>(string url, string requestType, object? requestBody = null, Dictionary<string, string>? headers = null, string? authUser = null, string? authPword = null) where T : new()
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

            if (!string.IsNullOrEmpty(authUser) && !string.IsNullOrEmpty(authPword))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{authUser}:{authPword}");
                var authValue = Convert.ToBase64String(byteArray);
                request.AddHeader("Authorization", $"Basic {authValue}");
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
                    code = "26",
                    description = response.Content
                };
                result = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(errorResponse));
            }
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                code = "99",
                description = ex.Message
            };
            result = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(errorResponse));
        }
        return result;
    }
}
