using System.Text.RegularExpressions;

namespace AuthServer.Extensions;

public static class HttpContextExtensions
{
    const string versionRegEx = @"^v\d.\d$";
    public static void SetRequestContext(this HttpContext context)
    {
        var requestContext = context.Features.Get<AuthRequestContext>();
        if (requestContext == null)
        {
            var request = context.Request;
            var path = request.Path.Value ?? string.Empty;
            var pathSegments = path.Split('/');
            var hasTenantId = TryTenantId(pathSegments, out Guid tenantId);
            var hasVersion = TryVersion(pathSegments, out float version);

            var siteName = $"{request.Scheme}://{request.Host.ToUriComponent()}";
            var issuer = hasTenantId ? $"{siteName}/tenantId" : siteName;
            requestContext = new AuthRequestContext
            {
                IPAddress = context.Connection.RemoteIpAddress != null
                    ? context.Connection.RemoteIpAddress.ToString()
                    : string.Empty,
                Path = path,
                TenantId = tenantId,
                HasTenantId = hasTenantId,
                Version = version,
                HasVersion = hasVersion,
                SiteName = siteName,
                Issuer = issuer
            };
            context.Features.Set(requestContext);
        }

        static bool TryTenantId(string[] pathSegments, out Guid tenantId)
        {
            tenantId = Guid.Empty;
            return pathSegments.Length > 0
                ? Guid.TryParse(pathSegments[1], out tenantId)
                : false;
        }

        static bool TryVersion(string[] pathSegments, out float version)
        {
            var versionString = pathSegments.FirstOrDefault(x => Regex.IsMatch(x, versionRegEx)) ?? "v1.0";
            return float.TryParse(versionString.Replace("v", string.Empty), out version);
        }
    }

    public static AuthRequestContext? GetRequestContext(this HttpContext context)
    {
        return context.Features.Get<AuthRequestContext>();
    }

    public static void SetTenantsContext(this HttpContext context)
    {
        if (context.GetTenantsContext() != null)
        {
            return;
        }

        var requestContext = context.Features.Get<AuthRequestContext>();
        if (requestContext != null && requestContext.HasTenantId)
        {
            var tenantSettings = GetTenantSettings(context, requestContext.TenantId);
            context.Features.Set(tenantSettings);
        }
    }

    private static TenantSettings GetTenantSettings(HttpContext context, Guid tenantId)
    {
        return context.RequestServices.GetService<ITenantsDataProvider>().GetTenantSettings(tenantId);
    }

    public static TenantSettings? GetTenantsContext(this HttpContext context)
    {
        return context.Features.Get<TenantSettings>();
    }

    static readonly List<string> ValidAuthPaths = new()
    {
        WellKnownConfig.V1Url,
        WellKnownConfig.V2Url,
        Token.V1Url,
        Token.V2Url,
        Authorize.V1Url,
        Authorize.V2Url,
        Login.V1Url,
        Login.V2Url,
        Logout.V1Url,
        Logout.V2Url
    };

    public static bool HasValidAuthPath(this HttpContext context)
    {
        var requestContext = context.Features.Get<AuthRequestContext>();
        if (requestContext != null && requestContext.HasTenantId)
        {
            var validAuthPath = requestContext.Path?.Replace(requestContext.TenantId.ToString(), UrlParams.tenantId) ?? string.Empty;
            return ValidAuthPaths.Contains(validAuthPath); 
        }
        return false;
    }

}