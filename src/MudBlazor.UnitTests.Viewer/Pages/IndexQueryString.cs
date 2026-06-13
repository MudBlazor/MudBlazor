namespace MudBlazor.UnitTests.Pages;

internal static class IndexQueryString
{
    public static bool TryGetSelectedComponentType(string url, Type[] availableComponentTypes, out Type? componentType)
    {
        componentType = null;

        var uri = new Uri(url);
        var query = uri.Query;

        if (string.IsNullOrEmpty(query))
        {
            return false;
        }

        if (query.StartsWith('?'))
        {
            query = query.Substring(1);
        }

        var queryParams = query.Split('&');

        foreach (var param in queryParams)
        {
            var parts = param.Split('=');
            if (parts.Length == 2)
            {
                var key = parts[0];
                var value = Uri.UnescapeDataString(parts[1]);

                if (string.Equals(key, "component", StringComparison.OrdinalIgnoreCase))
                {
                    componentType = availableComponentTypes.FirstOrDefault(t =>
                        t.Name.Equals(value, StringComparison.OrdinalIgnoreCase));

                    if (componentType != null)
                    {
                        return true;
                    }

                    break;
                }
            }
        }

        return false;
    }

    public static string CreateComponentUrl(string url, Type componentType)
    {
        var uri = new Uri(url);
        var baseUrl = uri.GetLeftPart(UriPartial.Path);

        // Create the new query string
        var newUrl = $"{baseUrl}?component={componentType.Name}";

        return newUrl;
    }
}
