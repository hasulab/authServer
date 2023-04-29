namespace AuthServer.Extentions;

public static class ObjectExtentions
{
    public static Dictionary<string,string> ToDictionary<T>(this T t)
        where T: class
    {
        if (t == null)
        {
            throw new ArgumentNullException(nameof(t));
        }
        var type = typeof(T);
        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return properties
            .Select(p => new { p, v = p.GetValue(t, null) })
            .Where(x => x.v != null)
            .ToDictionary(k => k.p.Name, v => v?.v?.ToString());
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    }

    public static string ToQueryString(this Dictionary<string, string> queryDictionary)
    {
        var queryString = string.Join('&', queryDictionary.Select(x => $"{x.Key}={x.Value}"));
        return queryString; 

    }
}