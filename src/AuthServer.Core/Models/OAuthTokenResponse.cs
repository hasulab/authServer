using System.Text.Json.Serialization;

namespace AuthServer.Models;

public class OAuthTokenResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string code { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string state { get; set; }

    public string token_type { get; set; } = TokenType.Bearer;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string expires_in { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ext_expires_in { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string expires_on { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string not_before { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string scope { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string id_token { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string resource { get; set; } = "00000002-0000-0000-c000-000000000000";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string access_token { get; set; }
    /*
     "token_type": "Bearer",
  "expires_in": "3599",
  "ext_expires_in": "3599",
  "expires_on": "1680540490",
  "not_before": "1680536590",
  "resource": "00000002-0000-0000-c000-000000000000",*/

}