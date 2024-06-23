namespace CryptoCoinApp.Helpers.Interface
{
    public interface IHttpConnectionHelper
    {
        Task<T> AppRequest<T>(
            string url, 
            string requestType, 
            object? requestBody = null, 
            Dictionary<string, string>? headers = null, 
            string? authUser = null, 
            string? authPword = null) where T : new();
    }
}
