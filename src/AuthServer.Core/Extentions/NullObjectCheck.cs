namespace AuthServer.Extentions;

public static class NullObjectCheck
{
    public static void ThrowAuthExceptionIfNull(this object obj, string errorCode, string errorDescription)
    {
        if (obj == null)
        {
            throw new AuthException
            {
                OAuthError = new OAuthErrorResponse
                {
                    error = errorCode,
                    error_description = errorDescription
                }
            };
        }
    }
}