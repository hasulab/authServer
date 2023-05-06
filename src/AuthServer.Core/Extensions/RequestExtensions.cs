using System.Text;
using System.Text.Json;

namespace AuthServer.Extensions;

public static class RequestExtensions
{
    public static async Task FormContentToJson(this HttpRequest request)
    {
        if (!request.HasFormContentType)
        {
            throw new Exception("Invalid ContentType");
        }
        var formFields = await request.ReadFormAsync();
        var enumerator = formFields.GetEnumerator();
        var hasMore = enumerator.MoveNext();

        StringBuilder stringBuilder = new("{");
        while (hasMore)
        {
            var field = enumerator.Current;
            stringBuilder.Append($"\"{field.Key}\":\"{field.Value}\"");
            hasMore = enumerator.MoveNext();
            if (hasMore)
            {
                stringBuilder.Append(',');
            }
        }
        stringBuilder.Append("}");

        request.ContentType = "application/json";
        request.Body = StreamExtensions.GenerateStreamFromStringBuilder(stringBuilder);
    }

    public static T QueryStringTo<T>(this HttpRequest request)
        where T:class, new()
    {
        if (!request.QueryString.HasValue)
        {
            return new T();
        }

        var queryString = request.QueryString.Value ?? string.Empty;

        var enumerator = StringExtensions.ToDictionary(queryString).GetEnumerator();
        var hasMore = enumerator.MoveNext();

        StringBuilder stringBuilder = new("{");
        while (hasMore)
        {
            var field = enumerator.Current;
            stringBuilder.Append($"\"{field.Key}\":\"{field.Value}\"");
            hasMore = enumerator.MoveNext();
            if (hasMore)
            {
                stringBuilder.Append(',');
            }
        }

        stringBuilder.Append('}');
        return JsonSerializer.Deserialize<T>(stringBuilder.ToString());        
    }
}