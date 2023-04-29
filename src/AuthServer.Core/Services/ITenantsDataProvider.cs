namespace AuthServer.Services;

///https://jasonwatmore.com/post/2021/06/02/net-5-create-and-validate-jwt-tokens-use-custom-jwt-middleware

public interface ITenantsDataProvider
{
    TenantSettings GetTenantSettings(Guid tenantId);
}