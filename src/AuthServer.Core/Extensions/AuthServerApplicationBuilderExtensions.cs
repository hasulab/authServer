using AuthServer.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace AuthServer.Extensions;

public static class AuthServerApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="AuthMiddleware"/> middleware to the specified <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to add the middleware to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// <para>
    /// A call to <see cref="UseAuthServer(WebApplication)"/> must be followed by a call to
    /// <see cref="UseAuthServer(WebApplication)"/> for the same <see cref="WebApplication"/>
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

        app.UseAuthMiddleware();
        //app.UseEndpoints();
        app.UseRouting();
        app.UseAuthEndpoints();
    }


    public static void UseAuthStaticFiles(this WebApplication app)
    {
        var currentFileProvider = app.Environment.ContentRootFileProvider as PhysicalFileProvider;

        //get app current ContentRootFileProvider

        var myFileProvider = new MyPhysicalFileProvider(currentFileProvider?.Root!, app?.Services?.GetService<IHttpContextAccessor>()!);
        app.Environment.ContentRootFileProvider = myFileProvider;

        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = myFileProvider
        });
    }

    public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
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
        app.MapGet("/{tenantId}/v2.0",
                (HttpRequest request, IAuthPageViewService viewService, string tenantId) =>
                {
                    return viewService.RenderHomePage(tenantId);
                })
            .WithName(AuthPage.HomePageV2);

        app.MapGet(WellKnownConfig.V1Url,
                (WellKnownConfiguration configuration, HttpRequest request, string tenantId) =>
                {
                    var siteName = $"{request.Scheme}://{request.Host.ToUriComponent()}";
                    return Results.Text(configuration.GetV1(siteName, tenantId), "application/json");
                })
            .WithName(WellKnownConfig.V1EPName);

        app.MapGet(WellKnownConfig.V2Url,
                (WellKnownConfiguration configuration, HttpRequest request, string tenantId) =>
                {
                    var siteName = $"{request.Scheme}://{request.Host.ToUriComponent()}";
                    return Results.Text(configuration.GetV2(siteName, tenantId), "application/json");
                })
            .WithName(WellKnownConfig.V2EPName);

        app.MapPost(Token.V1Url,
                (OAuth2Token tokenService, [FromBody] OAuthTokenRequest tokenRequest,
                    [FromServices] AuthRequestContext requestContext) =>
                {
                    return AuthResults.HandleAuhResponse(tokenRequest.response_mode,
                        () => tokenService.GenerateResponse(tokenRequest, requestContext));
                })
            .WithName(Token.V1EPName);

        app.MapPost(Token.V2Url,
                (OAuth2Token tokenService, [FromBody] OAuthTokenRequest tokenRequest,
                    [FromServices] AuthRequestContext requestContext) =>
                {
                    return AuthResults.HandleAuhResponse(tokenRequest.response_mode,
                        () => tokenService.GenerateResponse(tokenRequest, requestContext));
                })
            .WithName(Token.V2EPName);


        app.MapGet(Authorize.V1Url,
                (OAuth2Token tokenService, HttpRequest request, [FromServices] AuthRequestContext requestContext) =>
                {
                    var tokenRequest = request.QueryStringTo<OAuthTokenRequest>();
                    return AuthResults.HandleAuhResponse(() =>
                        tokenService.BuildAuthorizeResponse(tokenRequest, requestContext));
                })
            .WithName(Authorize.V1GetEPName);

        app.MapPost(Authorize.V1Url,
                (OAuth2Token tokenService, [FromBody] OAuthTokenRequest tokenRequest,
                    [FromServices] AuthRequestContext requestContext) =>
                {
                    return AuthResults.HandleAuhResponse(tokenRequest.response_mode ?? ResponseMode.fragment,
                        () => tokenService.GenerateResponse(tokenRequest, requestContext), tokenRequest.redirect_uri);
                })
            .WithName(Authorize.V1PostEPName);

        app.MapGet(Authorize.V2Url,
                (OAuth2Token tokenService, HttpRequest request, [FromServices] AuthRequestContext requestContext) =>
                {
                    var tokenRequest = request.QueryStringTo<OAuthTokenRequest>();
                    return AuthResults.HandleAuhResponse(() =>
                        tokenService.BuildAuthorizeResponse(tokenRequest, requestContext));
                })
            .WithName(Authorize.V2GetEPName);

        app.MapPost(Authorize.V2Url,
                (OAuth2Token tokenService, [FromBody] OAuthTokenRequest tokenRequest,
                    [FromServices] AuthRequestContext requestContext) =>
                {
                    return AuthResults.HandleAuhResponse(tokenRequest.response_mode ?? ResponseMode.fragment,
                        () => tokenService.GenerateResponse(tokenRequest, requestContext));
                })
            .WithName(Authorize.V2PostEPName);


        app.MapGet(Login.V1Url,
                (HttpRequest request, IAuthPageViewService viewService, string tenantId) =>
                {
                    return viewService.RenderLogin(tenantId);
                })
            .WithName(Login.V1GetEPName);

        app.MapGet(Login.V2Url,
                (HttpRequest request, [FromServices] AuthRequestContext requestContext, string tenantId) =>
                {
                    return Results.Ok();
                })
            .WithName(Login.V2GetEPName);

        app.MapGet(Logout.V1Url,
                (HttpRequest request, [FromServices] AuthRequestContext requestContext, string tenantId) =>
                {
                    return Results.Ok();
                })
            .WithName(Logout.V1GetEPName);

        app.MapGet(Logout.V2Url,
                (HttpRequest request, [FromServices] AuthRequestContext requestContext, string tenantId) =>
                {
                    return Results.Ok();
                })
            .WithName(Logout.V2GetEPName);



    }

}