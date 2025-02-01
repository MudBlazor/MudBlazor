// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Common;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents.ToggleGroup;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleGroupTests : BunitTest
    {
        [Test]
        public void ToggleGroup_Bind_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupBindTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ToggleItem() => comp.FindAll(".mud-toggle-item").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().BeNull();
            toggleSecond.Instance.Value.Should().BeNull();
            ToggleItem().Click();
            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Value.Should().Be("Item Two");
        }

        [Test]
        public void ToggleGroup_CustomFragmentBind_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupCustomFragmentTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ToggleItem() => comp.FindAll(".mud-toggle-item").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().BeNull();
            toggleSecond.Instance.Value.Should().BeNull();
            ToggleItem().Click();
            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Value.Should().Be("Item Two");
        }

        [Test]
        public void ToggleGroup_SelectionMode_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupBindMultiSelectionTest>();
            var group1 = comp.FindComponents<MudToggleGroup<string>>().First();
            var group2 = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ToggleItemSecond() => comp.FindAll(".mud-toggle-item").GetItemByIndex(1);
            IElement ToggleItemThird() => comp.FindAll(".mud-toggle-item").GetItemByIndex(2);

            group1.Instance.Values.Should().BeNull();
            group2.Instance.Values.Should().BeNull();
            ToggleItemSecond().Click();
            group1.Instance.Values.Should().Contain("Item Two");
            group2.Instance.Values.Should().Contain("Item Two");
            ToggleItemThird().Click();
            group1.Instance.Values.Should().BeEquivalentTo("Item Two", "Item Three");
            group2.Instance.Values.Should().Contain("Item Three");
            ToggleItemSecond().Click();
            group1.Instance.Values.Should().BeEquivalentTo("Item Three");
            group2.Instance.Values.Should().Contain("Item Three");
        }

        [Test]
        public void ToggleGroup_Initialize_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupInitializeTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();

            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Values.Should().BeEquivalentTo("Item One", "Item Three");

            comp.Find("#set-single-value").Click();
            toggleFirst.Instance.Value.Should().Be("Item One");

            comp.Find("#set-multi-value").Click();
            toggleSecond.Instance.Values.Should().BeEquivalentTo("Item Two", "Item Three");
        }

        [Test]
        public void ToggleGroup_ToggleSelection_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupToggleSelectionTest>();
            var toggle = comp.FindComponent<MudToggleGroup<string>>();
            IElement ToggleItem() => comp.FindAll(".mud-toggle-item").GetItemByIndex(0);

            toggle.Instance.Value.Should().BeNull();
            ToggleItem().Click();
            toggle.Instance.Value.Should().Be("Item One");
            ToggleItem().Click();
            toggle.Instance.Value.Should().BeNull();
        }

        [Test]
        public void ToggleGroup_ToggleRemove_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupRemoveTest>();
            var toggle = comp.FindComponent<MudToggleGroup<string>>();
            var toggleGroup = toggle.Instance;
            IElement Button() => comp.Find("#remove_btn");

            toggleGroup.GetItems().Count().Should().Be(8);
            Button().Click();
            toggleGroup.GetItems().Count().Should().Be(7);
        }

        [Test]
        [TestCase(Size.Small)]
        [TestCase(Size.Medium)]
        [TestCase(Size.Large)]
        public void ToggleGroup_SizeClasses_Test(Size size)
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Size, size);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });

            switch (size)
            {
                case Size.Small:
                    comp.FindAll(".mud-toggle-group-size-small").Count.Should().Be(1);
                    comp.FindAll(".mud-toggle-group-size-medium").Count.Should().Be(0);
                    comp.FindAll(".mud-toggle-group-size-large").Count.Should().Be(0);
                    break;
                case Size.Medium:
                    comp.FindAll(".mud-toggle-group-size-small").Count.Should().Be(0);
                    comp.FindAll(".mud-toggle-group-size-medium").Count.Should().Be(1);
                    comp.FindAll(".mud-toggle-group-size-large").Count.Should().Be(0);
                    break;
                case Size.Large:
                    comp.FindAll(".mud-toggle-group-size-small").Count.Should().Be(0);
                    comp.FindAll(".mud-toggle-group-size-medium").Count.Should().Be(0);
                    comp.FindAll(".mud-toggle-group-size-large").Count.Should().Be(1);
                    break;
            }
        }

        [Test(Description = "Ensures the checkmark is a direct descendant of the button label, is using the right name, and correctly contains a custom class definition")]
        public void ToggleGroup_CheckMarkClass()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.CheckMarkClass, "c69");
                builder.Add(x => x.CheckMark, true);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a").Add(x => x.UnselectedIcon, @Icons.Material.Filled.Coronavirus));
            });

            comp.Find(".mud-button-label > .mud-toggle-item-check-icon").ClassList.Should().Contain("c69");
        }

        [Test]
        public void ToggleGroup_ItemRegistration_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });
            comp.Instance.GetItems().Count().Should().Be(3);
            // re-registering an item won't do nothing
            comp.Instance.Register(comp.Instance.GetItems().First());
            comp.Instance.GetItems().Count().Should().Be(3);
        }

        [Test]
        public void ToggleGroup_SelectionModeWarning_Test()
        {
            var provider = new MockLoggerProvider();
            var logger = provider.CreateLogger(GetType().FullName!) as MockLogger;
            Context.Services.AddLogging(x => x.ClearProviders().AddProvider(provider)); //set up the logging provider
            foreach (var mode in new[] { SelectionMode.SingleSelection, SelectionMode.ToggleSelection })
            {
                Context.RenderComponent<MudToggleGroup<string>>(builder =>
                {
                    builder.Add(x => x.SelectionMode, mode);
                    builder.Add(x => x.ValuesChanged, new Action<IEnumerable<string>>(_ => { }));
                });
                logger!.GetEntries().Last().Level.Should().Be(LogLevel.Warning);
                logger.GetEntries().Last().Message.Should().Be($"For SelectionMode {mode} you should bind {nameof(MudToggleGroup<string>.Value)} instead of {nameof(MudToggleGroup<string>.Values)}");
            }
            Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.SelectionMode, SelectionMode.MultiSelection);
                builder.Add(x => x.ValueChanged, new Action<string>(_ => { }));
            });
            logger!.GetEntries().Last().Level.Should().Be(LogLevel.Warning);
            logger.GetEntries().Last().Message.Should().Be($"For SelectionMode {SelectionMode.MultiSelection} you should bind {nameof(MudToggleGroup<string>.Values)} instead of {nameof(MudToggleGroup<string>.Value)}");
            logger.GetEntries().Count.Should().Be(3);
            // no warning if both are bound
            Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.SelectionMode, SelectionMode.MultiSelection);
                builder.Add(x => x.ValueChanged, new Action<string>(_ => { }));
                builder.Add(x => x.ValuesChanged, new Action<IEnumerable<string>>(_ => { }));
            });
            logger.GetEntries().Count.Should().Be(3);
            // no warning if none are bound
            Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.SelectionMode, SelectionMode.MultiSelection);
            });
            logger.GetEntries().Count.Should().Be(3);
        }

        [Test]
        public void ToggleGroup_Disabled_Test()
        {
            var comp = Context.RenderComponent<ToggleGroupDisabledTest>();
            var toggleGroups = comp.FindComponents<MudToggleGroup<string>>();
            var disabledToggleGroup = toggleGroups[0];
            var enabledToggleGroup = toggleGroups[1];

            disabledToggleGroup.Instance.Disabled.Should().BeTrue();
            disabledToggleGroup.Find("div.mud-toggle-group").ClassList.Should().Contain("mud-disabled");
            foreach (var item in disabledToggleGroup.FindComponents<MudToggleItem<string>>())
            {
                // If the group is disabled, the group's disabled state overrules the item's disabled state
                item.Find("button.mud-toggle-item").HasAttribute("disabled").Should().BeTrue();
            }

            enabledToggleGroup.Instance.Disabled.Should().BeFalse();
            enabledToggleGroup.Find("div.mud-toggle-group").ClassList.Should().NotContain("mud-disabled");
            foreach (var item in enabledToggleGroup.FindComponents<MudToggleItem<string>>())
            {
                // If the group is enabled, the item's disabled state dominates
                item.Find("button.mud-toggle-item").HasAttribute("disabled").Should().Be(item.Instance.Disabled);
            }
        }

        [Test]
        [TestCase(SelectionMode.SingleSelection, "b")]
        [TestCase(SelectionMode.MultiSelection, "b")]
        [TestCase(SelectionMode.ToggleSelection, "b")]
        public void ToggleGroup_SetSelectedFromValuesTest(SelectionMode selMode, string selectedValues)
        {
            // Arrange
            var comp = Context.RenderComponent<MudToggleGroup<string>>(parameters => parameters
                .Add(p => p.SelectionMode, selMode)
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"))
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"))
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"))
                );

            var toggleGroup = comp.Instance;
            var items = toggleGroup.GetItems().ToList();

            // Act
            if (selMode == SelectionMode.MultiSelection)
            {
                comp.SetParametersAndRender(parameters => parameters.Add(p => p.Values, [selectedValues]));
            }
            else
            {
                comp.SetParametersAndRender(parameters => parameters.Add(p => p.Value, selectedValues));
            }

            // Assert
            // Verify only the selected item has the selected state
            items.Single(x => x.Value == selectedValues).Selected.Should().BeTrue();
            items.Where(x => x.Value != selectedValues).All(x => !x.Selected).Should().BeTrue();

            // Verify the UI reflects the selection
            comp.FindAll("button.mud-toggle-item-selected").Count.Should().Be(1);
            comp.Find("button.mud-toggle-item-selected").TextContent.Should().Contain(selectedValues);

            // Verify the internal state matches
            if (selMode == SelectionMode.MultiSelection)
            {
                toggleGroup.Values.Should().BeEquivalentTo([selectedValues]);
                toggleGroup.Value.Should().BeNull();
            }
            else
            {
                toggleGroup.Value.Should().Be(selectedValues);
                toggleGroup.Values.Should().BeNull();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void ToggleGroup_RTLTest(bool isRTL)
        {
            // Arrange
            var comp = Context.RenderComponent<MudToggleGroup<string>>(parameters => parameters
                .Add(p => p.RightToLeft, isRTL)
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"))
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"))
                );

            // Assert
            var group = comp.Find(".mud-toggle-group");

            if (isRTL)
            {
                group.ClassList.Should().Contain("mud-toggle-group-rtl");
            }
            else
            {
                group.ClassList.Should().NotContain("mud-toggle-group-rtl");
            }

        }

        [Test]
        public void ToggleGroup_VerticalTest()
        {
            // Arrange & Act
            var comp = Context.RenderComponent<MudToggleGroup<string>>(parameters => parameters
                .Add(p => p.Vertical, true)
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"))
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"))
                );

            // Assert
            var group = comp.Find(".mud-toggle-group");
            group.ClassList.Should().Contain("mud-toggle-group-vertical");
            group.ClassList.Should().NotContain("mud-toggle-group-horizontal");

            // Act - set to false
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Vertical, false));

            // Assert
            group = comp.Find(".mud-toggle-group");
            group.ClassList.Should().NotContain("mud-toggle-group-vertical");
            group.ClassList.Should().Contain("mud-toggle-group-horizontal");
        }

        [Test]
        public void ToggleGroup_FixedContentTest()
        {
            // Arrange & Act
            var comp = Context.RenderComponent<MudToggleGroup<string>>(parameters => parameters
                .Add(p => p.FixedContent, true)
                .Add(p => p.CheckMark, true)
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"))
                );

            // Assert - should have fixed content padding
            comp.Find(".mud-toggle-item").ClassList.Should().Contain("mud-toggle-item-fixed");

            // Act - disable fixed content
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FixedContent, false));

            // Assert
            comp.Find(".mud-toggle-item").ClassList.Should().NotContain("mud-toggle-item-fixed");
        }

        [Test]
        public void ToggleGroup_MultipleSelectionTest()
        {
            // Arrange
            var comp = Context.RenderComponent<MudToggleGroup<string>>(parameters => parameters
                .Add(p => p.SelectionMode, SelectionMode.MultiSelection)
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"))
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"))
                .AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"))
                );

            var toggleGroup = comp.Instance;
            var items = toggleGroup.GetItems().ToList();

            // Act - select multiple items
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Values, ["a", "c"]));

            // Assert
            // Verify correct items are selected
            items.First(x => x.Value == "a").Selected.Should().BeTrue();
            items.First(x => x.Value == "b").Selected.Should().BeFalse();
            items.First(x => x.Value == "c").Selected.Should().BeTrue();

            // Verify UI shows multiple selections
            var selectedButtons = comp.FindAll("button.mud-toggle-item-selected");
            selectedButtons.Count.Should().Be(2);
            selectedButtons[0].TextContent.Should().Contain("a");
            selectedButtons[1].TextContent.Should().Contain("c");

            // Verify internal state
            toggleGroup.Values.Should().BeEquivalentTo(["a", "c"]);
            toggleGroup.Value.Should().BeNull();

            // Act - deselect an item
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Values, ["a"]));

            // Assert
            items.First(x => x.Value == "a").Selected.Should().BeTrue();
            items.First(x => x.Value == "b").Selected.Should().BeFalse();
            items.First(x => x.Value == "c").Selected.Should().BeFalse();

            selectedButtons = comp.FindAll("button.mud-toggle-item-selected");
            selectedButtons.Count.Should().Be(1);
            selectedButtons[0].TextContent.Should().Contain("a");

            toggleGroup.Values.Should().BeEquivalentTo(["a"]);
            toggleGroup.Value.Should().BeNull();
        }
    }
}
