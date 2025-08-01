// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

public class ComboBoxTests : BunitTest
{

    [Test]
    public void ComboBox_ShouldRenderCorrectly()
    {
        var comp = Context.RenderComponent<MudComboBox<string>>(parameters => parameters
            .Add(p => p.OuterClass, "test-outer")
            .Add(p => p.InputClass, "test-input")
            );

        var container = comp.Find(".mud-select.mud-combobox");
        container.Should().NotBeNull();
        container.Id.Should().Be($"{comp.Instance._elementId}");
        container.ClassList.Should().Contain("test-outer");

        if (comp.Instance.FullWidth)
            container.ClassList.Should().Contain("mud-width-full");
        else
            container.ClassList.Should().NotContain("mud-width-full");

        var input = comp.Find(".mud-select input.mud-select-input");
        input.Should().NotBeNull();

        input.ClassList.Should().Contain("test-input");

    }
}

