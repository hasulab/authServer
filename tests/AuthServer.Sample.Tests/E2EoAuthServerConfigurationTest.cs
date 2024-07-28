using System.Net;

namespace AuthServer.Sample.Tests;

public class E2EoAuthServerConfigurationTest : E2EoAuthServerTestBase
{
    [Fact]
    public async Task TestGetConfiguration()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var url = BuildAuthUrl(Constants.Auth.WellKnownConfig.V1Url);
        var response = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
