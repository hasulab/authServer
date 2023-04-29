namespace AuthServer.Models;

public record class AuthRequestContext
{
    public string IPAddress { get; internal set; }
    public string Path { get; internal set; }
    public Guid TenantId { get; internal set; }
    public bool HasTenantId { get; internal set; }
    public float Version { get; internal set; }
    public bool HasVersion { get; internal set; }
    public string SiteName { get; internal set; }
    public string Issuer { get; internal set; }
    public DateTimeOffset RequestTime { get; internal set; } = DateTimeOffset.UtcNow;

}