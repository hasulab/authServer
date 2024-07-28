using System.Net;

namespace AuthServer.Sample.Tests;

public class E2EoAuthServerAuthorizeTest: E2EoAuthServerTestBase
{
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