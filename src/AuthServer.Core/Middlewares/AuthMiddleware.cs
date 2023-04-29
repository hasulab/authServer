using System.Diagnostics;

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
        return _next(httpContext);
    }
}
