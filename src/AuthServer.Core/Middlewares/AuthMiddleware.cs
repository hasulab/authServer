using System.Diagnostics;
using AuthServer.Extensions;

namespace AuthServer.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(ILogger<AuthMiddleware> logger,
        DiagnosticListener diagnosticListener,
        RequestDelegate next)
    {
        _next = next;
    }
    public Task Invoke(HttpContext httpContext)
    {
        httpContext.SetRequestContext();

        if (httpContext.HasValidAuthPath())
        {
            return AwaitRequestTask(httpContext);
        }

        return _next(httpContext);

        static async Task AwaitRequestTask(HttpContext httpContext)
        {
            if (httpContext.Request.HasFormContentType)
            {
                await httpContext.Request.FormContentToJson();
            }

            httpContext.SetTenantsContext();
            await Task.CompletedTask;
        }
    }
}
