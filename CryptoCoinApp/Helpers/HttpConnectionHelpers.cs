using CryptoCoinApp.Helpers.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoCoinApp.Helpers
{
    public class HttpConnectionHelpers : IHttpConnectionHelpers
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
            T? result = new();
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(requestType.ToLower() == "post" ? Method.Post : Method.Get);

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
}
