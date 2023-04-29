using System.Security.Claims;

namespace AuthServer.Services;

public interface IJwtUtils
{
    public string GenerateToken(ClaimsIdentity claimsIdentity, AuthRequestContext requestContext,
        out long expiresIn,
        DateTime? issuedAt = null, DateTime? notBefore = null, DateTime? expires = null, double defaultExpiryMinutes = 30);
    public bool ValidateToken(string token, AuthRequestContext requestContext);
}