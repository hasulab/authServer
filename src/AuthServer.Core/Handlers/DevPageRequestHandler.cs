using MediatR;

namespace AuthServer.Handlers;

public class DevPageRequestHandler : IRequestHandler<DevPageRequestHandler.Request, IResult>
{
    private readonly LinkGenerator _linker;
    private readonly IAuthPageViewService _viewService;

    public DevPageRequestHandler(LinkGenerator linker, IAuthPageViewService viewService)
    {
        _linker = linker;
        _viewService = viewService;
    }
    public Task<IResult> Handle(Request request, CancellationToken cancellationToken)
    {
        var v2HomePage = _linker.GetPathByName(AuthPage.HomePageV2, values: new { tenantId = Guid.Empty });
        return Task.FromResult(Results.Redirect(v2HomePage));
    }

    public class Request : IRequest<IResult>
    {

    }
}