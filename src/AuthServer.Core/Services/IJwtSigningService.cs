using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services;

public interface IJwtSigningService
{
    SecurityKey GetSecurityKey(Guid tenantId);
    SigningCredentials GetSigningCredentials(Guid tenantId);
    EncryptingCredentials GetEncryptingCredentials(Guid tenantId);
}