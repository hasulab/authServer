using System.Text;

namespace AuthServer.Extentions;

public static class StringExtentions
{ 
    public static string ToBase64String(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var base64EncodedText = Convert.ToBase64String(plainTextBytes);
        return base64EncodedText;
    }

    public static Dictionary<string, string> ToDictionary(this string queryString)
    {
        var queryDictionary = queryString.Split('&')
            .Select(x =>
            {
                var kv = x.Split('=');
                return new { k = kv.First(), v = kv.Last() };
            }).
            ToDictionary(x => x.k, x => x.v);

        return queryDictionary;
    }
}