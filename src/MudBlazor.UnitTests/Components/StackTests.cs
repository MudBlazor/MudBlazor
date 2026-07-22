using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class StackTests : BunitTest
    {
        [Test]
        public void DefaultValues()
        {
            var stack = new MudStack();

            stack.Row.Should().BeFalse();
            stack.Reverse.Should().BeFalse();
            stack.Spacing.Should().Be(3);
            stack.Justify.Should().BeNull();
            stack.AlignItems.Should().BeNull();
            stack.StretchItems.Should().BeNull();
        }

        [Test]
        public void CheckDefaultClass()
        {
            var stack = Context.RenderComponent<MudStack>();

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", "gap-3" });
        }

        [Test]
        public void CheckRowClass()
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.Row, true));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-row", "gap-3" });
        }

        [Test]
        public void CheckReverseClass()
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.Reverse, true));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column-reverse", "gap-3" });
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        public void CheckSpacingClass(int spacing)
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.Spacing, spacing));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"gap-{spacing}" });
        }

        [Test]
        [TestCase(Justify.FlexStart, "start")]
        [TestCase(Justify.Center, "center")]
        [TestCase(Justify.FlexEnd, "end")]
        [TestCase(Justify.SpaceBetween, "space-between")]
        [TestCase(Justify.SpaceAround, "space-around")]
        [TestCase(Justify.SpaceEvenly, "space-evenly")]
        public void CheckJustifyClass(Justify justify, string expectedClass)
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.Justify, justify));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"justify-{expectedClass}", "gap-3" });
        }

        [Test]
        [TestCase(AlignItems.Baseline, "baseline")]
        [TestCase(AlignItems.Center, "center")]
        [TestCase(AlignItems.Start, "start")]
        [TestCase(AlignItems.End, "end")]
        [TestCase(AlignItems.Stretch, "stretch")]
        public void CheckAlignItemsClass(AlignItems align, string expectedClass)
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.AlignItems, align));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"align-{expectedClass}", "gap-3" });
        }


        [Test]
        [TestCase(StretchItems.Start, "start")]
        [TestCase(StretchItems.End, "end")]
        [TestCase(StretchItems.StartAndEnd, "start-and-end")]
        [TestCase(StretchItems.Middle, "middle")]
        [TestCase(StretchItems.All, "all")]
        public void CheckStretchItemsClass(StretchItems stretch, string expectedClass)
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.StretchItems, stretch));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().Contain(["d-flex", $"flex-grow-{expectedClass}"]);
        }

        [Test]
        public void CheckStretchItemsNoneClass()
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.StretchItems, StretchItems.None));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().NotContain(["flex-grow-start", "flex-grow-end", "flex-grow-start-and-end", "flex-grow-all"]);
        }

        [Test]
        [TestCase(Wrap.NoWrap, "nowrap")]
        [TestCase(Wrap.Wrap, "wrap")]
        [TestCase(Wrap.WrapReverse, "wrap-reverse")]
        public void CheckWrapClass(Wrap wrap, string expectedClass)
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.Wrap, wrap));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"flex-{expectedClass}", "gap-3" });
        }
    }
}



