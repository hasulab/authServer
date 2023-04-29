namespace AuthServer.Services;

public interface IAuthPageViewService
{
    IResult RenderHomePage(string? tenantId);
    IResult RenderLogin(string tenantId);
}