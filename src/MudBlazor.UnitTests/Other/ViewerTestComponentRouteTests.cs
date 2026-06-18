using System.Reflection;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class ViewerTestComponentRouteTests
    {
        // Mirrors the discovery filter in MudBlazor.UnitTests.Viewer/Pages/Index.razor (ordering is irrelevant here).
        private static List<Type> GetViewerComponentTypes()
        {
            var assembly = Assembly.Load("MudBlazor.UnitTests.Viewer");

            return assembly.GetTypes()
                .Where(type => type.Name.Contains("Test"))
                .Where(type => !type.Name.StartsWith("<"))
                .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
                .ToList();
        }

        // Mirrors RouteKey(...) in the viewer's Index.razor.
        private static string RouteKey(Type type)
        {
            var ns = type.Namespace ?? string.Empty;
            const string marker = "TestComponents.";
            var index = ns.IndexOf(marker, StringComparison.Ordinal);
            var folder = index >= 0 ? ns[(index + marker.Length)..].Replace('.', '/') : string.Empty;

            return string.IsNullOrEmpty(folder) ? type.Name : $"{folder}/{type.Name}";
        }

        [Test]
        public void ViewerComponentRouteKeysAreUnique()
        {
            var components = GetViewerComponentTypes();

            // Guard against silently-broken discovery making the uniqueness check pass vacuously.
            components.Should().NotBeEmpty();

            // The viewer resolves /viewer/<route-key> by path, so duplicate route keys would be unreachable.
            var duplicates = components
                .GroupBy(RouteKey, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => $"{group.Key} ({group.Count()})")
                .ToList();

            duplicates.Should().BeEmpty();
        }
    }
}
