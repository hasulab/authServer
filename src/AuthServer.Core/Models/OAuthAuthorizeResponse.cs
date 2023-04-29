namespace AuthServer.Models;

public class OAuthAuthorizeResponse
{
    public string LoginUrl { get; set; }
    public string RequestToken { get; set; }
}