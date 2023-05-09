namespace AuthServer.Services;

internal class WellKnownConfiguration
{
    private readonly ResourceReader _resourceReader;

    public WellKnownConfiguration(ResourceReader resourceReader)
    {
        _resourceReader = resourceReader;
    }

    public string GetV1(string endpoint, string tenantId)
    {
        return GetResourceText(WellKnownConfig.V1ConfigresourceName, endpoint, tenantId);
    }

    public string GetV2(string endpoint, string tenantId)
    {
        return GetResourceText(WellKnownConfig.V2ConfigresourceName, endpoint, tenantId);
    }

    private string GetResourceText(string resourceName, string endpoint, string tenantId)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            throw new ArgumentException($"'{nameof(resourceName)}' cannot be null or empty.", nameof(resourceName));
        }

        if (string.IsNullOrEmpty(endpoint))
        {
            throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or empty.", nameof(endpoint));
        }

        if (string.IsNullOrEmpty(tenantId))
        {
            throw new ArgumentException($"'{nameof(tenantId)}' cannot be null or empty.", nameof(tenantId));
        }
        return _resourceReader.GetStringFromResource(resourceName)
            .Replace("__AUTH_ENDPOINT__", endpoint)
            .Replace("__AUTH_UID__", tenantId);
    }
}