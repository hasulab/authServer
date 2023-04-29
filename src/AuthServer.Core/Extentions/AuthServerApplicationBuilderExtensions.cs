using AuthServer.Middlewares;

namespace AuthServer.Extentions;

public static class AuthServerApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="EndpointRoutingMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// <para>
    /// A call to <see cref="UseRouting(IApplicationBuilder)"/> must be followed by a call to
    /// <see cref="UseEndpoints(IApplicationBuilder, Action{IEndpointRouteBuilder})"/> for the same <see cref="IApplicationBuilder"/>
    /// instance.
    /// </para>
    /// <para>
    /// The <see cref="EndpointRoutingMiddleware"/> defines a point in the middleware pipeline where routing decisions are
    /// made, and an <see cref="Endpoint"/> is associated with the <see cref="HttpContext"/>. The <see cref="EndpointMiddleware"/>
    /// defines a point in the middleware pipeline where the current <see cref="Endpoint"/> is executed. Middleware between
    /// the <see cref="EndpointRoutingMiddleware"/> and <see cref="EndpointMiddleware"/> may observe or change the
    /// <see cref="Endpoint"/> associated with the <see cref="HttpContext"/>.
    /// </para>
    /// </remarks>
    public static void UseAuthServer(this WebApplication app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.UseAuthMiddlewares();
        app.UseAuthEndpoints();
    }

    public static IApplicationBuilder UseAuthMiddlewares(this IApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        builder.UseMiddleware<AuthMiddleware>();

        return builder;
    }

    public static void UseAuthEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapGet("/{tenantId}/v2.0", (HttpRequest request,  string tenantId) =>
        {
            return Results.Ok("OK");
        })
    .WithName(AuthPage.HomePageV2);



    }

}