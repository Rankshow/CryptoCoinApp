namespace CryptoCoinApp.Common;
public class ApiResponse
{
    public List<CryptoRecord> data { get; set; }
    public long timestamp { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
}

