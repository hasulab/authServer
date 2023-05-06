namespace AuthServer.Extensions;

public static class ServiceProviderExtensions
{
    public static T? GetHttpContextFeature<T>(this IServiceProvider serviceProvider)
        where T: class, new()
    {
        var HttpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        if (HttpContextAccessor?.HttpContext?.Features != null)
        {
            return HttpContextAccessor?.HttpContext.Features.Get<T>();
        }
        else
            return null;
    }
}