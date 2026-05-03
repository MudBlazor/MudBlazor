// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

[TestFixture]
public sealed class ApiLinkServiceTests
{
    [TestCase("Explore", "docs/overview")]
    [TestCase("Installation", "getting-started/installation")]
    [TestCase("Roadmap", "mud/project/roadmap")]
    public async Task Search_Returns_Navigation_Pages(string searchText, string expectedLink)
    {
        var service = CreateService();

        var results = await service.Search(searchText);

        results.Should().Contain(entry => entry.Link == expectedLink);
    }

    [TestCase("Getting Started", "getting-started/installation")]
    [TestCase("Get Started", "getting-started/installation")]
    [TestCase("Learn More", "mud/introduction")]
    public async Task Search_Returns_Navigation_Aliases(string searchText, string expectedLink)
    {
        var service = CreateService();

        var results = await service.Search(searchText);

        results.Should().Contain(entry => entry.Link == expectedLink);
    }

    [Test]
    public void GetAllEntries_Includes_Navigation_Pages()
    {
        var service = CreateService();

        var entries = service.GetAllEntries();

        entries.Should().Contain(entry => entry.Link == "docs/overview" && entry.Title == "Explore");
        entries.Should().Contain(entry => entry.Link == "getting-started/layouts" && entry.Title == "Layouts");
        entries.Should().Contain(entry => entry.Link == "mud/project/roadmap" && entry.Title == "Roadmap");
    }

    private static global::MudBlazor.Docs.Services.ApiLinkService CreateService()
    {
        return new global::MudBlazor.Docs.Services.ApiLinkService(new global::MudBlazor.Docs.Services.MenuService());
    }
}
