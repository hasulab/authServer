namespace AuthServer.Models;

public class OAuthTokenRequest
{
    public static bool TryParse(string queryString, out OAuthTokenRequest tokenRequest)
    {
        tokenRequest = null;
        if (string.IsNullOrEmpty(queryString))
        {
            return false;
        }
        var queryDictionary = queryString.Split('&')
            .Select(x =>
            {
                var kv = x.Split('=');
                return new { k = kv.First(), v = kv.Last() };
            }).ToDictionary(x => x.k, x => x.v);

        tokenRequest = new OAuthTokenRequest();

        foreach (var kv in queryDictionary)
        {
            if (_updateToken.ContainsKey(kv.Key))
            {
                _updateToken[kv.Key](tokenRequest, kv.Value);
            }
        }

        return true;

    }
    delegate void UpdateToken(OAuthTokenRequest tokenRequest, string value);
    private static readonly Dictionary<string, UpdateToken> _updateToken = new()
    {
        {nameof(client_id), (req,value)=> { req.client_id = value; } },
        {nameof(client_secret), (req,value)=> { req.client_secret = value; } },
        {nameof(username), (req,value)=> { req.username = value; } },
        {nameof(password), (req,value)=> { req.password = value; } },
        {nameof(client_assertion), (req,value)=> { req.client_assertion = value; } },
        {nameof(code_verifier), (req,value)=> { req.code_verifier = value; } },
        {nameof(redirect_uri), (req,value)=> { req.redirect_uri = value; } },
        {nameof(code), (req,value)=> { req.code = value; } },
        {nameof(scope), (req,value)=> { req.scope = value; } },
        {nameof(tenant), (req,value)=> { req.tenant = value; } },
        {nameof(response_type), (req,value)=> { req.response_type = value; } },
        {nameof(response_mode), (req,value)=> { req.response_mode = value; } },
        {nameof(state), (req,value)=> { req.state = value; } },
        {nameof(nonce), (req,value)=> { req.nonce = value; } },
        {nameof(prompt), (req,value)=> { req.prompt = value; } },
        {nameof(login_hint), (req,value)=> { req.login_hint = value; } },
        {nameof(code_challenge), (req,value)=> { req.code_challenge = value; } },
        {nameof(code_challenge_method), (req,value)=> { req.code_challenge_method = value; } }
    };

    public string grant_type { get; set; } = GrantType.client_credentials;

    //client_id is AppId in  jwt
    public string client_id { get; set; }
    public string client_secret { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string client_assertion { get; set; }
    public string code_verifier { get; set; }
    public string redirect_uri { get; set; }
    public string code { get; set; }
    public string scope { get; set; }
    public string tenant { get; set; }
    public string response_type { get; set; } //code id_token access_token
    public string response_mode { get; set; }//query,fragment,form_post
    public string state { get; set; }
    public string nonce { get; set; }
    public string prompt { get; set; }
    public string login_hint { get; set; }
    public string code_challenge { get; set; }
    public string code_challenge_method { get; set; }
}