using System.Net;

namespace AuthServer.Sample.Tests;
public class E2EoAuthServerSwaggerPageTest : E2EoAuthServerTestBase
{
    [Fact]
    public async Task TestSwaggerPage()
    {
        await using var application = new TestAuthWebApplication();

        var client = application.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
