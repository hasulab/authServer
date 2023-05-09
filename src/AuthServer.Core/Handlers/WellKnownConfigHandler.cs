using MediatR;

namespace AuthServer.Handlers;

internal class WellKnownConfigHandler : IRequestHandler<WellKnownConfigHandler.V1Request, IResult>,
    IRequestHandler<WellKnownConfigHandler.V2Request, IResult>
{
    private readonly WellKnownConfiguration _configuration;

    public WellKnownConfigHandler(WellKnownConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Task<IResult> Handle(V1Request request, CancellationToken cancellationToken)
    {
        var siteName = $"{request.Request.Scheme}://{request.Request.Host.ToUriComponent()}";
        return Task.FromResult(Results.Text(_configuration.GetV1(siteName, request.TenantId), "application/json"));
    }

    public Task<IResult> Handle(V2Request request, CancellationToken cancellationToken)
    {
        var siteName = $"{request.Request.Scheme}://{request.Request.Host.ToUriComponent()}";
        return Task.FromResult(Results.Text(_configuration.GetV2(siteName, request.TenantId), "application/json"));
    }
    internal record V1Request(string TenantId, HttpRequest Request) : IRequest<IResult>;
    internal record V2Request(string TenantId, HttpRequest Request) : IRequest<IResult>;

}