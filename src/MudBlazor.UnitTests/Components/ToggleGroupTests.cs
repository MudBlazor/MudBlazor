// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Common;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public async Task ToggleGroup_HorizontalItemPadding_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Dense, false);
                builder.Add(x => x.Rounded, false);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-2");
                item.ClassList.Should().Contain("py-2");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Dense, true));
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-1");
                item.ClassList.Should().Contain("py-1");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Rounded, true));
            IElement Item1() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(0);
            IElement Item2() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            IElement Item3() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);
            // (x|_|_)
            Item1().ClassList.Should().Contain("ps-2");
            Item1().ClassList.Should().Contain("pe-1");
            Item1().ClassList.Should().Contain("py-1");
            // (_|X|_)
            Item2().ClassList.Should().Contain("px-1");
            Item2().ClassList.Should().Contain("py-1");
            // (_|_|x)
            Item3().ClassList.Should().Contain("pe-2");
            Item3().ClassList.Should().Contain("ps-1");
            Item3().ClassList.Should().Contain("py-1");
            await comp.InvokeAsync(() => comp.SetParam(x => x.Dense, false));
            // (x|_|_)
            Item1().ClassList.Should().Contain("ps-3");
            Item1().ClassList.Should().Contain("pe-2");
            Item1().ClassList.Should().Contain("py-2");
            // (_|X|_)
            Item2().ClassList.Should().Contain("px-2");
            Item2().ClassList.Should().Contain("py-2");
            // (_|_|x)
            Item3().ClassList.Should().Contain("pe-3");
            Item3().ClassList.Should().Contain("ps-2");
            Item3().ClassList.Should().Contain("py-2");
        }

        [Test]
        public async Task ToggleGroup_VerticalItemPadding_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Dense, false);
                builder.Add(x => x.Rounded, false);
                builder.Add(x => x.Vertical, true);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-2");
                item.ClassList.Should().Contain("py-2");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Dense, true));
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-1");
                item.ClassList.Should().Contain("py-1");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Rounded, true));
            IElement Item1() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(0);
            IElement Item2() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            IElement Item3() => comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);
            // top (x|_|_) bottom
            Item1().ClassList.Should().Contain("pt-2");
            Item1().ClassList.Should().Contain("pb-1");
            Item1().ClassList.Should().Contain("px-1");
            // top (_|X|_) bottom
            Item2().ClassList.Should().Contain("px-1");
            Item2().ClassList.Should().Contain("py-1");
            // top (_|_|x) bottom
            Item3().ClassList.Should().Contain("pb-2");
            Item3().ClassList.Should().Contain("pt-1");
            Item3().ClassList.Should().Contain("px-1");
            await comp.InvokeAsync(() => comp.SetParam(x => x.Dense, false));
            // top (x|_|_) bottom
            Item1().ClassList.Should().Contain("pt-3");
            Item1().ClassList.Should().Contain("pb-2");
            Item1().ClassList.Should().Contain("px-2");
            // top (_|X|_) bottom
            Item2().ClassList.Should().Contain("px-2");
            Item2().ClassList.Should().Contain("py-2");
            // top (_|_|x) bottom
            Item3().ClassList.Should().Contain("pb-3");
            Item3().ClassList.Should().Contain("pt-2");
            Item3().ClassList.Should().Contain("px-2");
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
            icon.ClassList.Should().Contain("me-2"); // <--- the spacing between icon and text
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
                builder.Add(x => x.Dense, false);
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
    }
}
