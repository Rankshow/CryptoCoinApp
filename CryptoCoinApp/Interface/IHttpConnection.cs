namespace CryptoCoinApp.Interface
{
    public interface IHttpConnection
    {
        Task<T> WebRequest<T>(
            string url,
            string requestType,
            object? requestBody = null,
            Dictionary<string, string>? headers = null,
            string? authUser = null,
            string? authPassword = null) where T : new();
    }
}
