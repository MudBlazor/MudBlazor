// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.Docs.Examples;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Generated;

public partial class ExampleDocsTests
{
    /// <summary>
    /// The column-order example gives all four keyboard-reachable order selectors distinct accessible names.
    /// </summary>
    [Test]
    public void DataGridColumnOrderStateExampleNamesOrderSelectors()
    {
        var component = _ctx.Render<DataGridColumnOrderStateExample>();
        var selectors = component.FindAll("[role='combobox']");

        selectors.Should().HaveCount(4);
        selectors.Select(x => x.GetAttribute("aria-label"))
            .Should()
            .Equal("Select column order", "Name column order", "Age column order", "Role column order");

        foreach (var selector in selectors)
        {
            selector.GetAttribute("tabindex").Should().Be("0");
            selector.GetAttribute("aria-haspopup").Should().Be("listbox");
            selector.GetAttribute("aria-expanded").Should().Be("false");
        }
    }
}
