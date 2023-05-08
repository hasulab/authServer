using Microsoft.AspNetCore.Hosting;

namespace AuthServer.Extensions;

public static class AuthServerServiceCollectionExtensions
{
    /// <summary>
    /// Adds a default implementation for the <see cref="IHttpContextAccessor"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAuthServerServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services
             .AddHttpContextAccessor()
             .AddSingleton<ResourceReader>()
             .AddScoped<WellKnownConfiguration>()
             .AddScoped<OAuth2Token>()
             .AddScoped<IJwtSigningService, JwtSigningService>()
             .AddScoped<IJwtUtils, JwtUtils>()
             .AddScoped<ClientDataProvider>()
             .AddScoped<ITenantsDataProvider, TenantsDataProvider>()
             .AddScoped<IAuthPageViewService, AuthPageViewService>()
             //    .AddScoped<IAuthPageViewService, AuthPageViewService> ()
             .AddScoped<AuthRequestContext>((sp) =>
             {
                 return sp.GetHttpContextFeature<AuthRequestContext>() ?? new AuthRequestContext();
             })
             .AddScoped<TenantSettings>((sp) =>
             {
                 return sp.GetHttpContextFeature<TenantSettings>() ?? new TenantSettings();
             })
             ;

        services
            .AddOptions<AuthSettings>().BindConfiguration("AuthSettings");

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<JwtSigningService>());
        return services;
    }
}
