using System.Reflection;
using AwesomeAssertions;
using MudBlazor.Docs.Models;
using MudBlazor.Examples.Data.Models;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Utilities;

[TestFixture]
public class ModelSnippetTests
{
    /// <summary>
    /// Verifies that the data model code sent to TryMud declares every property and object override of the real model (issue #12881).
    /// </summary>
    [TestCase(typeof(Element))]
    [TestCase(typeof(Server))]
    [TestCase(typeof(Company))]
    public void ModelSnippetMatchesModel(Type modelType)
    {
        var snippet = Snippets.GetCode(modelType.Name);

        snippet.Should().Contain($"class {modelType.Name}");

        foreach (var property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            snippet.Should().Contain($" {property.Name} {{ get; set; }}");
        }

        var overrides = modelType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => method.GetBaseDefinition().DeclaringType == typeof(object));
        foreach (var method in overrides)
        {
            snippet.Should().MatchRegex($@"override \S+ {method.Name}\(");
        }
    }
}
