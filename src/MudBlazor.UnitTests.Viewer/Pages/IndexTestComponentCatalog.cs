using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.Pages;

internal static class IndexTestComponentCatalog
{
    public static IEnumerable<Type> GetTestComponentTypes()
    {
        var types = typeof(Program).Assembly.GetTypes()
            .Where(type => type.Name.Contains("Test"))
            .Where(type => !type.Name.StartsWith("<"))
            .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
            .OrderBy(type => type.Name);

        foreach (var type in types)
        {
            yield return type;
        }
    }

    public static TestEntry CreateEntry(Type type)
    {
        var category = type.Namespace?.Split('.').LastOrDefault() ?? string.Empty;
        var description = GetDescriptionOrNull(type);
        var filePath = GetFilePath(type);
        return new TestEntry(type, type.Name, GetDisplayName(type.Name), category, description, filePath);
    }

    public static string GetFilePath(Type type)
    {
        var ns = type.Namespace ?? string.Empty;
        var match = "MudBlazor.UnitTests.";
        var path = ns.StartsWith(match) ? ns.Substring(match.Length).Replace('.', '/') : ns.Replace('.', '/');
        return $"{path}/{type.Name}.razor";
    }

    public static string GetDisplayName(string typeName)
    {
        return typeName.EndsWith("Test", StringComparison.Ordinal)
            ? typeName.Substring(0, typeName.Length - 4)
            : typeName;
    }

    public static string? GetDescriptionOrNull(Type type)
    {
        var field = type.GetField("__description__", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
        if (field is null || field.FieldType != typeof(string))
        {
            return null;
        }

        return (string?)field.GetValue(null);
    }
}

internal sealed record TestEntry(Type Type, string Name, string DisplayName, string Category, string? Description, string FilePath);
