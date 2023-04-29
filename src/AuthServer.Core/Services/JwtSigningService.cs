using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services;

internal class JwtSigningService : IJwtSigningService
{
    private readonly TenantSettings _tenantSettings;

    public JwtSigningService(TenantSettings tenantSettings)
    {
        _tenantSettings = tenantSettings;
    }

    public SecurityKey GetSecurityKey(Guid tenantId)
    {
        SecurityKey secret = null;
        if (_tenantSettings.CertificateFile != null && _tenantSettings.CertificatePassword != null )
        {
            var cert = new X509Certificate2(_tenantSettings.CertificateFile, _tenantSettings.CertificatePassword);
            secret = new X509SecurityKey(cert);
        }
        else if (_tenantSettings.SecretKey != null)
        {
            secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tenantSettings.SecretKey));
        }
        else
        {
            _tenantSettings?.SecretKey.ThrowAuthExceptionIfNull(Errors.invalid_resource, "SecretKey or CertificateFile not configured");
        }
        return secret;
    }

    public EncryptingCredentials GetEncryptingCredentials(Guid tenantId)
    {
        var secret = GetSecurityKey(tenantId);
        var encryptionCredentials = new EncryptingCredentials(secret, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
        return encryptionCredentials;
    }

    public SigningCredentials GetSigningCredentials(Guid tenantId)
    {
        var secret = GetSecurityKey(tenantId);
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        //using a certificate file
        //X509Certificate2 cert = new X509Certificate2("MySelfSignedCertificate.pfx", "password");
        //X509SecurityKey key = new X509SecurityKey(cert);
        //signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        return signingCredentials;
    }

}