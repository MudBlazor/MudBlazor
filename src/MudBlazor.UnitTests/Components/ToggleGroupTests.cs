// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleGroupTests : BunitTest
    {
        [Test]
        public void ToggleGroup_Bind_Test()
        {
            var comp = Context.RenderComponent<ToggleBindTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ToggleItem() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().BeNull();
            toggleSecond.Instance.Value.Should().BeNull();
            ToggleItem().Click();
            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Value.Should().Be("Item Two");
        }

        [Test]
        public void ToggleGroup_CustomFragmentBind_Test()
        {
            var comp = Context.RenderComponent<ToggleCustomFragmentTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ToggleItem() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().BeNull();
            toggleSecond.Instance.Value.Should().BeNull();
            ToggleItem().Click();
            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Value.Should().Be("Item Two");
        }

        [Test]
        public void ToggleGroup_SelectionMode_Test()
        {
            var comp = Context.RenderComponent<ToggleBindMultiSelectionTest>();
            var group1 = comp.FindComponents<MudToggleGroup<string>>().First();
            var group2 = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ToggleItemSecond() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            IElement ToggleItemThird() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);

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
            var comp = Context.RenderComponent<ToggleInitializeTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            IElement ButtonOne() => comp.FindAll("button").GetItemByIndex(0);
            IElement ButtonTwo() => comp.FindAll("button").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Values.Should().BeEquivalentTo("Item One", "Item Three");

            ButtonOne().Click();
            toggleFirst.Instance.Value.Should().Be("Item One");

            ButtonTwo().Click();
            toggleSecond.Instance.Values.Should().BeEquivalentTo("Item Two", "Item Three");
        }

        [Test]
        public void ToggleGroup_ToggleSelection_Test()
        {
            var comp = Context.RenderComponent<ToggleToggleSelectionTest>();
            var toggle = comp.FindComponent<MudToggleGroup<string>>();
            IElement ToggleItem() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(0);

            toggle.Instance.Value.Should().BeNull();
            ToggleItem().Click();
            toggle.Instance.Value.Should().Be("Item One");
            ToggleItem().Click();
            toggle.Instance.Value.Should().BeNull();
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

        [Test]
        public void ToggleGroup_CustomClasses_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.CheckMarkClass, "c69");
                builder.Add(x => x.TextClass, "c42");
                builder.Add(x => x.CheckMark, true);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a").Add(x => x.UnselectedIcon, @Icons.Material.Filled.Coronavirus));
            });
            var icon = comp.Find("svg");
            icon.ClassList.Should().Contain("c69");
            var text = comp.Find(".mud-typography");
            text.ClassList.Should().Contain("c42");
        }

        [Test]
        public void ToggleItem_IsEmpty_Test()
        {
#pragma warning disable BL0005
            new MudToggleItem<string>() { Text = null, Value = null }.IsEmpty.Should().Be(true);
            new MudToggleItem<string>() { Text = "", Value = null }.IsEmpty.Should().Be(true);
            new MudToggleItem<string>() { Text = "a", Value = null }.IsEmpty.Should().Be(false);
            new MudToggleItem<string>() { Text = null, Value = "a" }.IsEmpty.Should().Be(false);
#pragma warning restore BL0005
        }

        [Test]
        public void ToggleGroup_ItemRegistration_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Rounded, false);
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
            var comp = Context.RenderComponent<ToggleDisabledTest>();
            var toggleGroups = comp.FindComponents<MudToggleGroup<string>>();
            var disabledToggleGroup = toggleGroups[0];
            var enabledToggleGroup = toggleGroups[1];

            disabledToggleGroup.Instance.Disabled.Should().BeTrue();
            disabledToggleGroup.Find("div.mud-toggle-group").ClassList.Should().Contain("mud-disabled");
            foreach (var item in disabledToggleGroup.FindComponents<MudToggleItem<string>>())
            {
                // If the group is disabled, the group's disabled state overrules the item's disabled state
                item.Find("div.mud-toggle-item").ClassList.Should().Contain("mud-disabled");
            }

            enabledToggleGroup.Instance.Disabled.Should().BeFalse();
            enabledToggleGroup.Find("div.mud-toggle-group").ClassList.Should().NotContain("mud-disabled");
            foreach (var item in enabledToggleGroup.FindComponents<MudToggleItem<string>>())
            {
                // If the group is enabled, the item's disabled state dominates
                if (item.Instance.Disabled)
                {
                    item.Find("div.mud-toggle-item").ClassList.Should().Contain("mud-disabled");
                }
                else
                {
                    item.Find("div.mud-toggle-item").ClassList.Should().NotContain("mud-disabled");
                }
            }
        }
    }
}
