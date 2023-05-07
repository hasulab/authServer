using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using AuthServer.Extentions;
using AuthServer.Models;
using AuthServer.Services;

namespace AuthServer.Tests;

public class OAuth2TokenTest
{
    [Fact]
    public void TestGenerateToken()
    {
        var settings = new TenantSettings
        {
            SecretKey = "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeyS"
        };
        var jwtSigningService = new JwtSigningService(settings);
        var util = new JwtUtils(jwtSigningService);
        AuthRequestContext requestContext = new();
        var token = util.GenerateToken(new ClaimsIdentity(new List<Claim> { new Claim("Id", "TestId") }), requestContext,
            out long expiresIn);

    }

    [Fact]
    public void TestGenerateAccessTokenFromRequest()
    {
        var moqHttpContext = new Mock<HttpContext>();
        var moqHttpRequest = new Mock<HttpRequest>();
        var moqFeatures = new Mock<IFeatureCollection>();
        var moqClientDataProvider = new Mock<ClientDataProvider>();
        var features = new TestFeatureCollection();


        moqHttpContext.Setup(x => x.Request).Returns(() => moqHttpRequest.Object);
        moqHttpContext.Setup(x => x.Features).Returns(() => features);

        moqHttpRequest.Setup(x => x.Path).Returns("/10000000-0000-0000-0000-000000000001/v2.0/.well-known/openid-configuration");
        moqHttpRequest.Setup(x => x.Scheme).Returns("https");
        moqHttpRequest.Setup(x => x.Host).Returns(new HostString("testserver"));

        var ipAddr = new byte[4] { 192, 168, 255, 251 };
        moqHttpContext.Setup(x => x.Connection.RemoteIpAddress).Returns(() => new System.Net.IPAddress(ipAddr));

        moqClientDataProvider
            .Setup(x => x.ValidateSecret(It.IsAny<Guid>(), It.IsAny<OAuthTokenRequest>()))
            .Returns(() => new ClientDataProvider.StoredUser { Id = "TestId1" });
        moqClientDataProvider
            .Setup(x => x.GetResponseTypes(It.IsAny<Guid>(), It.IsAny<OAuthTokenRequest>()))
            .Returns(() => new string[] { Constants.Auth.ResponseType.access_token });

        HttpContextExtensions.SetRequestContext(moqHttpContext.Object);
        var requestContext = HttpContextExtensions.GetRequestContext(moqHttpContext.Object);

        var settings = new TenantSettings
        {
            SecretKey = "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeyS"
        };
        var jwtSigningService = new JwtSigningService(settings);
        var util = new JwtUtils(jwtSigningService);
        TenantSettings tenantSettings = new()
        {

        };

        var service = new OAuth2Token(util, moqClientDataProvider.Object, tenantSettings);
        var tokenRequest = new OAuthTokenRequest()
        {
            client_id = "TestClient id",
            grant_type = Constants.Auth.GrantType.client_credentials,
            client_secret = "SuperS3cr3t"
        };
        var jwtToken = service.GenerateResponse(tokenRequest, requestContext);

    }

    [Fact]
    public void TestGenerateIdTokenFromRequest()
    {
        var moqHttpContext = new Mock<HttpContext>();
        var moqHttpRequest = new Mock<HttpRequest>();
        var moqFeatures = new Mock<IFeatureCollection>();
        var moqClientDataProvider = new Mock<ClientDataProvider>();
        var features = new TestFeatureCollection();

        moqHttpContext.Setup(x => x.Request).Returns(() => moqHttpRequest.Object);
        moqHttpContext.Setup(x => x.Features).Returns(() => features);

        moqHttpRequest.Setup(x => x.Path).Returns("/10000000-0000-0000-0000-000000000001/v2.0/.well-known/openid-configuration");
        moqHttpRequest.Setup(x => x.Scheme).Returns("https");
        moqHttpRequest.Setup(x => x.Host).Returns(new HostString("testserver"));

        var ipAddr = new byte[4] { 192, 168, 255, 251 };
        moqHttpContext.Setup(x => x.Connection.RemoteIpAddress).Returns(() => new System.Net.IPAddress(ipAddr));

        moqClientDataProvider
            .Setup(x => x.ValidateUserPassword(It.IsAny<Guid>(), It.IsAny<OAuthTokenRequest>()))
            .Returns(() => new ClientDataProvider.StoredUser
            {
                Id = "TestId1",
                Email = "test@test.com",
                Name = "Testname",
                Roles = new string[] { "Admin", "Test1Role", "Test2Role" }
            });

        moqClientDataProvider
            .Setup(x => x.GetResponseTypes(It.IsAny<Guid>(), It.IsAny<OAuthTokenRequest>()))
            .Returns(() => new string[] { Constants.Auth.ResponseType.id_token });

        HttpContextExtensions.SetRequestContext(moqHttpContext.Object);
        var requestContext = HttpContextExtensions.GetRequestContext(moqHttpContext.Object);

        var settings = new TenantSettings
        {
            SecretKey = "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeyS"
        };
        var jwtSigningService = new JwtSigningService(settings);
        var util = new JwtUtils(jwtSigningService);
        TenantSettings tenantSettings = new()
        {

        };
        var service = new OAuth2Token(util, moqClientDataProvider.Object, tenantSettings);
        var tokenRequest = new OAuthTokenRequest()
        {
            client_id = "TestClient id",
            grant_type = Constants.Auth.GrantType.password,
            scope = "email,openId",
            username = "test",
            password = "P@ssword"
        };
        var jwtToken = service.GenerateResponse(tokenRequest, requestContext);

    }
}