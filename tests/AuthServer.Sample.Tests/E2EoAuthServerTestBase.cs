namespace AuthServer.Sample.Tests
{
    public class E2EoAuthServerTestBase
    {
        protected static string BuildAuthUrl(string url, Guid? tenantId = null)
        {
            tenantId = Guid.Empty;
            return url.Replace("{tenantId}", tenantId.ToString());
        }
    }
}