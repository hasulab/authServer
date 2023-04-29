using Microsoft.Extensions.Options;

namespace AuthServer.Services;

public class TenantsDataProvider : ITenantsDataProvider
{
    AuthSettings _authSettings;
    public TenantsDataProvider(IOptions<AuthSettings> authOptions)
    {
        _authSettings = authOptions.Value;
    }

    public TenantSettings GetTenantSettings(Guid tenantId)
    {
        return _authSettings.Tenants
            .SingleOrDefault(x => x.Id == tenantId) ?? new TenantSettings();
    }
}