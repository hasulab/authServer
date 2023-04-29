namespace AuthServer.Services;

public class TenantSettings
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SecretKey { get; set; }
    public string CertificateFile { get; set; }
    public string CertificatePassword { get; set; }
    public string LoginUrl { get; set; }
    public string LogoutUrl { get; set; }
}