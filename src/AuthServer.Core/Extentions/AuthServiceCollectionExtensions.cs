using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthServer.Extentions;

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

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        return services;
    }
}
