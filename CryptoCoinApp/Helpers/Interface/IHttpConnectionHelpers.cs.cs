namespace CryptoCoinApp.Helpers.Interface
{
    public interface IHttpConnectionHelpers
    {
        Task<T> AppRequest<T>(
            string url,
            string requestType,
            object? requestBody = null,
            Dictionary<string, string>? headers = null,
            string? user = null,
            string? password = null) where T : new();
    }
}
