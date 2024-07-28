using System.Net;
using System.Net.Http.Json;
using AuthServer.Models;

namespace AuthServer.Sample.Tests;

public class E2EoAuthServerGenerateTokenTest : E2EoAuthServerTestBase
{

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
}
