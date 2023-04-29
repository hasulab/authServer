using System.Security.Claims;

namespace AuthServer.Services;

public class OAuth2Token
{
    private readonly IJwtUtils _jwtUtils;
    private readonly ClientDataProvider _clientDataProvider;
    private readonly TenantSettings _tenantSettings;

    delegate ClaimRecord claimBuilder(string name, AuthRequestContext request, AuthUser user);

    private static readonly Dictionary<string, claimBuilder> _claimBuilders = new()
    {
        {Claims.sub, (name, req, user)=> { return new ClaimRecord(name, user.UserId??user.Id); } },
        {Claims.oid, (name, req, user)=> { return new ClaimRecord(name, user.Id); } },
        {Claims.name, (name, req, user)=> { return new ClaimRecord(name, user.Name); } },
        {Claims.family_name, (name, req, user)=> { return new ClaimRecord(name, user.FamilyName); } },
        {Claims.given_name, (name, req, user)=> { return new ClaimRecord(name, user.GivenName); } },
        {Claims.email, (name, req, user)=> { return new ClaimRecord(name, user.Email); } },
        {Claims.unique_name, (name, req, user)=> { return new ClaimRecord(name, user.Email); } },
        {Claims.preferred_username, (name, req, user)=> { return new ClaimRecord(name, user.Email); } },
        {Claims.nonce, (name, req, user)=> { return new ClaimRecord(name, user.Nonce); } },
        //{Claims.roles, (name, req, user)=> { return new ClaimRecord(name, user.Roles); } },
        {Claims.appid, (name, req, user)=> { return new ClaimRecord(name, user.AppId ?? user.ClientId); } },
        {Claims.aud, (name, req, user)=> { return new ClaimRecord(name, user.AppId ?? user.ClientId); } },

        {Claims.tid, (name, req, user)=> { return new ClaimRecord(name, req.TenantId.ToString()); } },
        {Claims.ver, (name, req, user)=> { return new ClaimRecord(name, req.Version.ToString("F1")); } },
        //{Claims.iss, (name, req, user)=> { return new ClaimRecord(name, req.Issuer); } },
        {Claims.idp, (name, req, user)=> { return new ClaimRecord(name, req.Issuer); } },
    };

    record ClaimRecord
    {
        public ClaimRecord(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get;  }
        public string Value { get;}
    }

    internal static IEnumerable<Claim> BuildClaims(string[] includeClaims, AuthRequestContext request, AuthUser user)
    {
        if (includeClaims?.Length > 0)
        {
            var claims = includeClaims
                .Where(x => _claimBuilders.ContainsKey(x))
                .Select(x => _claimBuilders[x](x, request, user))
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToList();

            if (includeClaims.Contains(Claims.roles) && user.Roles?.Length > 0)
            {
                claims.AddRange(user.Roles.Select(x => new ClaimRecord(Claims.roles, x))); 
            }

            return claims.Select(x => new Claim(x.Key, x.Value)).ToList();
        }

        return new List<Claim>();
    }

    public OAuth2Token(IJwtUtils jwtUtils, ClientDataProvider clientDataProvider, TenantSettings tenantSettings)
    {
        _jwtUtils = jwtUtils;
        _clientDataProvider = clientDataProvider;
        _tenantSettings = tenantSettings;
    }

    delegate void UpdateToken(IJwtUtils jwtUtils, OAuthTokenResponse response, AuthUser authUser, AuthRequestContext requestCtx);

    private readonly Dictionary<string, UpdateToken> _tokenUpdaters = new()
    {
        { ResponseType.token,(jwtUtils, res,user, ctx)=> { UpdateAccessToken(jwtUtils, res,user,ctx); } },
        { ResponseType.access_token,(jwtUtils, res,user, ctx)=> { UpdateAccessToken(jwtUtils, res,user,ctx); } },
        { ResponseType.id_token,(jwtUtils, res,user, ctx)=> { UpdateIdToken(jwtUtils, res,user,ctx); } }
    };
    
    public OAuthAuthorizeResponse BuildAuthorizeResponse(OAuthTokenRequest tokenRequest,
        AuthRequestContext requestCtx)
    {

        var tokenRequestQuery = tokenRequest.ToDictionary().ToQueryString();
        var encodedToken = tokenRequestQuery.ToBase64String();
        var loginUrl = _tenantSettings.LoginUrl?.Replace(UrlParams.tenantId, requestCtx.TenantId.ToString());
        return new OAuthAuthorizeResponse
        {
            LoginUrl = loginUrl,
            RequestToken = encodedToken
        };
    }

    public OAuthTokenResponse GenerateResponse(OAuthTokenRequest tokenRequest, AuthRequestContext requestCtx)
    {
        AuthUser authUser;
        if (tokenRequest.grant_type == GrantType.client_credentials)
        {
            authUser = BuildAccessToken(tokenRequest, requestCtx);
        }
        else if (tokenRequest.grant_type == GrantType.password)
        {
            authUser = BuildIdToken(tokenRequest, requestCtx);
        }
        else
        {
            throw new AuthException
            {
                OAuthError = new OAuthErrorResponse
                {
                    error= Errors.invalid_grant,
                    error_description= $"Invalid {tokenRequest.grant_type} grant_type"
                }
            };
        }

        var responseTypes = _clientDataProvider.GetResponseTypes(requestCtx.TenantId ,tokenRequest);
        var tokenResponse = new OAuthTokenResponse();
        responseTypes
            .Where(x => _tokenUpdaters.ContainsKey(x))
            .ToList()
            .ForEach(x => _tokenUpdaters[x](_jwtUtils, tokenResponse, authUser, requestCtx));

        return tokenResponse;
    }

    private AuthUser BuildIdToken(OAuthTokenRequest tokenRequest, AuthRequestContext requestCtx)
    {
        var user = _clientDataProvider.ValidateUserPassword(requestCtx.TenantId, tokenRequest);

        user.ThrowAuthExceptionIfNull(Errors.invalid_request, "Invalid username or password");

        return new AuthUser
        {
            AppId = tokenRequest.client_id,
            Id = user.Id,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            FamilyName = user.SurName,
            GivenName = user.FirstName,
            Roles = user.Roles,
            ClientId = tokenRequest.client_id,
        };
    }

    private AuthUser BuildAccessToken(OAuthTokenRequest tokenRequest, AuthRequestContext requestCtx)
    {
        var user = _clientDataProvider.ValidateSecret(requestCtx.TenantId, tokenRequest);

        user.ThrowAuthExceptionIfNull(Errors.invalid_request, "Invalid token or client id");

        return new AuthUser
        {
            AppId = tokenRequest.client_id,
            Id = user.Id,
            UserId = user.Id,
            ClientId = tokenRequest.client_id,
        };
    }

    static readonly string[] AccesskenClaims = new string[] {
        Claims.aud, Claims.iss, Claims.idp, Claims.oid, Claims.sub, Claims.tid, Claims.ver
    };

    private static void UpdateAccessToken(IJwtUtils jwtUtils, OAuthTokenResponse response, AuthUser authUser, AuthRequestContext requestCtx)
    {
        var claims = BuildClaims(AccesskenClaims, requestCtx, authUser);
        var claimsIdentity= new ClaimsIdentity(claims);
        response.access_token = jwtUtils.GenerateToken(claimsIdentity, requestCtx, out long expiresIn);
        if (response.expires_in == null)
        {
            response.expires_in = expiresIn.ToString();
            response.ext_expires_in = expiresIn.ToString();
        }
    }

    static readonly string[] IdTokenClaims = new string[]
    {
        Claims.aud, Claims.iss, Claims.iat, Claims.nbf, Claims.exp, Claims.aio, Claims.amr, Claims.rsa,
        Claims.name, Claims.email, Claims.family_name, Claims.given_name, Claims.idp, Claims.roles,
        Claims.ipaddr,Claims.nonce, Claims.oid, Claims.rh, Claims.sub, Claims.tid, Claims.unique_name, Claims.uti, Claims.ver
    };

    private static void UpdateIdToken(IJwtUtils jwtUtils, OAuthTokenResponse response, AuthUser authUser, AuthRequestContext requestCtx)
    {
        var claims = BuildClaims(IdTokenClaims, requestCtx, authUser);
        var claimsIdentity = new ClaimsIdentity(claims);
        response.id_token = jwtUtils.GenerateToken(claimsIdentity, requestCtx, out long expiresIn);
        if (response.expires_in != null)
        {
            response.expires_in = expiresIn.ToString();
            response.ext_expires_in = expiresIn.ToString();
        }
    }
}