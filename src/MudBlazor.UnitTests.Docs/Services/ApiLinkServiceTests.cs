using AwesomeAssertions;
using MudBlazor.Docs.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

[TestFixture]
public sealed class ApiLinkServiceTests
{
    [Test]
    public async Task Search_FindsUtilityPagesByTheirGroupHierarchy()
    {
        var service = new ApiLinkService(new MenuService());

        var results = await service.Search("flex");

        results.Should().Contain(entry => entry.Link == "utilities/align-items" && entry.SubTitle == "CSS Utilities > Flexbox");
    }

    [Test]
    public async Task Search_DisambiguatesDuplicateTitlesWithGroupSubtitles()
    {
        var service = new ApiLinkService(new MenuService());

        var results = await service.Search("z-index");

        results.Should().Contain(entry => entry.Link == "customization/z-index" && entry.SubTitle == "Customization");
        results.Should().Contain(entry => entry.Link == "utilities/z-index" && entry.SubTitle == "CSS Utilities > Layout");
    }
}
