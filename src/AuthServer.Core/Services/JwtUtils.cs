using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services;

public class JwtUtils : IJwtUtils
{
    private readonly IJwtSigningService _signingService;

    public JwtUtils(IJwtSigningService signingService)
    {
        _signingService = signingService;
    }

    public string GenerateToken(ClaimsIdentity claimsIdentity, AuthRequestContext requestContext,
        out long expiresIn, DateTime? issuedAt = null, DateTime? notBefore = null, DateTime? expires = null,
        double defaultExpiryMinutes = 30)
    {
        // generate token that is valid for 30 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingCredentials = _signingService.GetSigningCredentials(requestContext.TenantId);

        var requestTime = requestContext.RequestTime;// DateTimeOffset.UtcNow;

        notBefore ??= requestTime.UtcDateTime;
        issuedAt ??= requestTime.UtcDateTime;
        expires ??= requestTime.UtcDateTime.AddMinutes(defaultExpiryMinutes);

        expiresIn = (long)(expires.Value - DateTime.UtcNow).TotalSeconds;

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = requestContext.Issuer,
            //Audience = requestContext.Issuer,
            Subject = claimsIdentity,
            NotBefore = notBefore,
            IssuedAt = issuedAt,
            Expires = expires,
            //EncryptingCredentials = encryptionCredentials,
            SigningCredentials = signingCredentials
        };
        var tokenOptions = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return tokenHandler.WriteToken(tokenOptions);
    }

    public bool ValidateToken(string token, AuthRequestContext requestContext)
    {
        if (token == null)
            return false;

        var secret = _signingService.GetSecurityKey(requestContext.TenantId);
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = requestContext.Issuer,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuer = true,
                //ValidAudience = requestContext.Issuer,
                //ValidateAudience = true,
                IssuerSigningKey = secret,
                TokenDecryptionKey = secret,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            //var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // return user id from JWT token if validation successful
            return true;
        }
        catch
        {
            // return null if validation fails
            return false;
        }
    }
}