using MediatR;

namespace AuthServer.Handlers;
internal class HomePageRequestHandler : IRequestHandler<HomePageRequestHandler.Request, IResult>
{
    private readonly LinkGenerator _linker;
    private readonly IAuthPageViewService _viewService;

    public HomePageRequestHandler(LinkGenerator linker, IAuthPageViewService viewService)
    {
        _linker = linker;
        _viewService = viewService;
    }
    public Task<IResult> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_viewService.RenderHomePage(request.TenantId));
    }

    internal record Request(string TenantId) : IRequest<IResult>;
}