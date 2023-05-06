using System.Text;
using AuthServer.Extensions;

namespace AuthServer.Services;

public class AuthPageViewService: IAuthPageViewService
{
    private readonly AuthRequestContext requestConext;
    private readonly ResourceReader resourceReader;
    private readonly LinkGenerator linker;

    public AuthPageViewService(AuthRequestContext requestConext,
        ResourceReader resourceReader,
        LinkGenerator linker)
    {
        this.requestConext = requestConext;
        this.resourceReader = resourceReader;
        this.linker = linker;
    }

    public IResult RenderHomePage(string? tenantId)
    {
        tenantId = tenantId ?? Guid.Empty.ToString();

        var links = new Dictionary<string, string?>()
            {
                { "V1 /.well-known/openid-configuration",linker.GetPathByName(WellKnownConfig.V1EPName, values: new { tenantId }) },
                { "V2 /.well-known/openid-configuration",linker.GetPathByName(WellKnownConfig.V2EPName, values: new { tenantId }) },
                { "V1 /oauth2/token",linker.GetPathByName(Token.V1EPName, values: new { tenantId }) },
                { "V2 /oauth2/token",linker.GetPathByName(Token.V2EPName, values: new { tenantId }) },
                { "V1 /oauth2/authorize",linker.GetPathByName(Authorize.V1GetEPName, values: new { tenantId }) },
                { "V2 /oauth2/authorize",linker.GetPathByName(Authorize.V2GetEPName, values: new { tenantId }) },
            }
            .Select(x => $"<a href='{x.Value}'>{x.Key}</a>").ToArray();

        var submitPath = linker.GetPathByName(Token.V1EPName, values: new { tenantId });

        var sbHtml = new StringBuilder(resourceReader.GetStringFromResource(IndexPage.V1ResourceName))
            .Replace(AuthPage.TenantId, tenantId)
            .Replace(AuthPage.PostLoginPath, submitPath)
            .Replace(AuthPage.Links, string.Join('\n', links))
            .Replace(AuthPage.ClientId, tenantId); //TODO: ClientId from request

        return ResultsExtensions.Html(sbHtml.ToString());
    }
    public IResult RenderLogin(string tenantId)
    {
        var submitPath = linker.GetPathByName(Token.V1EPName, values: new { tenantId });

        var sbHtml = new StringBuilder(resourceReader.GetStringFromResource(Login.V1ResourceName))
            .Replace(AuthPage.TenantId, tenantId)
            .Replace(AuthPage.PostLoginPath, submitPath)
            .Replace(AuthPage.ClientId, tenantId); //TODO: ClientId from request

        return ResultsExtensions.Html(sbHtml.ToString());
    }
}