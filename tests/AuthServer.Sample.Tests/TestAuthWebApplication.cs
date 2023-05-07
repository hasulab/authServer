using AuthServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AuthServer.Sample.Tests;

public class TestAuthWebApplication : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        builder.ConfigureServices(sp =>
        {
            sp.TryAddScoped<TenantSettings>(s => new TenantSettings
            {
                LoginUrl = "/auth/login"
            });
        });
        return base.CreateServer(builder);
    }
}