using System.Net;
using System.Net.Http.Json;
using AuthServer.Models;

namespace AuthServer.Sample.Tests;

public class E2EoAuthServerTest
{
    private static string BuildAuthUrl(string url, Guid? tenantId = null)
    {
        tenantId = Guid.Empty;
        return url.Replace("{tenantId}", tenantId.ToString());
    }

    [Fact]
    public async Task TestSwaggerPage()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestGetConfiguration()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var url = BuildAuthUrl(Constants.Auth.WellKnownConfig.V1Url);
        var response = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestGenerateToken()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var url = BuildAuthUrl(Constants.Auth.Token.V1Url);
        using var httpContent = new MultipartFormDataContent
        {
            { new StringContent("TestCId"), "client_id" },
            { new StringContent("S3cr3t"), "client_secret" }
        };
        var response = await client.PostAsync(url, httpContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestGenerateTokenWithJson()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var url = BuildAuthUrl(Constants.Auth.Token.V1Url);
        var jsonPayload = new OAuthTokenRequest
        {
            client_id = "TestCId",
            client_secret = "S3cret"
        };
        var response = await client.PostAsJsonAsync(url, jsonPayload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestGetAuthorize()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var url = BuildAuthUrl(Constants.Auth.Authorize.V1Url);
        var queryString = "?client_id=6731de76-14a6-49ae-97bc-6eba6914391e&response_type=id_token&redirect_uri=http%3A%2F%2Flocalhost%2Fmyapp%2F&scope=openid&response_mode=fragment&state=12345&nonce=678910";
        var response = await client.GetAsync(url + queryString);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestPostAuthorize()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var url = BuildAuthUrl(Constants.Auth.Authorize.V1Url);
        var queryString = "?client_id=6731de76-14a6-49ae-97bc-6eba6914391e&response_type=id_token&redirect_uri=http%3A%2F%2Flocalhost%2Fmyapp%2F&scope=openid&response_mode=fragment&state=12345&nonce=678910";
        using var httpContent = new MultipartFormDataContent
        {
            { new StringContent("TestCId"), "client_id" },
            { new StringContent("S3cr3t"), "client_secret" }
        };
        var response = await client.PostAsync(url + queryString, httpContent);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}