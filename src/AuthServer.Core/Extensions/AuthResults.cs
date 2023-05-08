namespace AuthServer.Extensions;

public static class AuthResults
{
    public static IResult HandleAuhResponse(Func<OAuthAuthorizeResponse> func)
    {
        try
        {
            var authorizeResponse = func();
            return BuildAuthResponse(ResponseMode.fragment, new { authorizeResponse.RequestToken }, authorizeResponse.LoginUrl);
        }
        catch (AuthException ex)
        {
            return Results.BadRequest(ex?.OAuthError);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    public static IResult HandleAuhResponse(string response_mode, Func<OAuthTokenResponse> func,
        string? redirect_uri = null)
    {
        try
        {
            var tokenResponse = func();
            return BuildAuthResponse(response_mode, tokenResponse, redirect_uri);
        }
        catch (AuthException ex)
        {
            return Results.BadRequest(ex?.OAuthError);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    private static IResult BuildAuthResponse<T>(string response_mode, T response, string? redirect_uri = null)
        where T : class
    {
        if (string.IsNullOrEmpty(response_mode))
        {
            return Results.Ok(response);
        }
        else switch (response_mode)
        {
            case ResponseMode.fragment:
            {
                var queryString = response.ToDictionary().ToQueryString();

                return Results.Redirect($"{redirect_uri}?{queryString}");
            }
            case ResponseMode.form_post:
                return Results.Ok(response);
            default:
                throw new AuthException(Errors.invalid_resource, $"invalid {nameof(response_mode)} : {response_mode}");
        }
    }
}