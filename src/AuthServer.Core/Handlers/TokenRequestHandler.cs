using AuthServer.Extensions;
using MediatR;

namespace AuthServer.Handlers;

internal class TokenRequestHandler : IRequestHandler<TokenRequestHandler.Request, IResult>
{
    private readonly OAuth2Token _tokenService;
    private readonly AuthRequestContext _requestContext;

    public TokenRequestHandler(OAuth2Token tokenService, AuthRequestContext requestContext)
    {
        _tokenService = tokenService;
        _requestContext = requestContext;
    }
    public Task<IResult> Handle(Request request, CancellationToken cancellationToken)
    {
        var response= AuthResults.HandleAuhResponse(request.TokenRequest.response_mode,
            () => _tokenService.GenerateResponse(request.TokenRequest, _requestContext));
        return Task.FromResult(response);
    }

    internal record Request(string TenantId, OAuthTokenRequest TokenRequest) : IRequest<IResult>;
}