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
        [TestCase(Breakpoint.None)]
        [TestCase(Breakpoint.Always)]
        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        [TestCase(Breakpoint.Md)]
        [TestCase(Breakpoint.Lg)]
        [TestCase(Breakpoint.Xl)]
        [TestCase(Breakpoint.Xxl)]
        [TestCase(Breakpoint.SmAndDown)]
        [TestCase(Breakpoint.MdAndDown)]
        [TestCase(Breakpoint.LgAndDown)]
        [TestCase(Breakpoint.XlAndDown)]
        [TestCase(Breakpoint.SmAndUp)]
        [TestCase(Breakpoint.MdAndUp)]
        [TestCase(Breakpoint.LgAndUp)]
        [TestCase(Breakpoint.XlAndUp)]
        [TestCase(Breakpoint.None, true)]
        [TestCase(Breakpoint.Always, true)]
        [TestCase(Breakpoint.Xs, true)]
        [TestCase(Breakpoint.Sm, true)]
        [TestCase(Breakpoint.Md, true)]
        [TestCase(Breakpoint.Lg, true)]
        [TestCase(Breakpoint.Xl, true)]
        [TestCase(Breakpoint.Xxl, true)]
        [TestCase(Breakpoint.SmAndDown, true)]
        [TestCase(Breakpoint.MdAndDown, true)]
        [TestCase(Breakpoint.LgAndDown, true)]
        [TestCase(Breakpoint.XlAndDown, true)]
        [TestCase(Breakpoint.SmAndUp, true)]
        [TestCase(Breakpoint.MdAndUp, true)]
        [TestCase(Breakpoint.LgAndUp, true)]
        [TestCase(Breakpoint.XlAndUp, true)]
        [TestCase(Breakpoint.None, true, true)]
        [TestCase(Breakpoint.Always, true, true)]
        [TestCase(Breakpoint.Xs, true, true)]
        [TestCase(Breakpoint.Sm, true, true)]
        [TestCase(Breakpoint.Md, true, true)]
        [TestCase(Breakpoint.Lg, true, true)]
        [TestCase(Breakpoint.Xl, true, true)]
        [TestCase(Breakpoint.Xxl, true, true)]
        [TestCase(Breakpoint.SmAndDown, true, true)]
        [TestCase(Breakpoint.MdAndDown, true, true)]
        [TestCase(Breakpoint.LgAndDown, true, true)]
        [TestCase(Breakpoint.XlAndDown, true, true)]
        [TestCase(Breakpoint.SmAndUp, true, true)]
        [TestCase(Breakpoint.MdAndUp, true, true)]
        [TestCase(Breakpoint.LgAndUp, true, true)]
        [TestCase(Breakpoint.XlAndUp, true, true)]
        public void CheckBreakpointClass(Breakpoint breakpoint, bool row = false, bool reverse = false)
        {
            var stack = Context.RenderComponent<MudStack>(x => x.Add(c => c.Breakpoint, breakpoint).Add(c => c.Row, row).Add(c => c.Reverse, reverse));

            // Get the Default and Reverse States
            string defaultState = (row ? "row" : "column") + (reverse ? "-reverse" : string.Empty);
            string reverseState = (row ? "column" : "row") + (reverse ? "-reverse" : string.Empty);

            // Get the Stack Class
            var stackClass = stack.Find(".d-flex");

            // Handle Special Cases
            switch (breakpoint)
            {
                // If the Breakpoint is None or Always, return the default direction
                case Breakpoint.None: // If breakpoint is None, return the default direction 
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.Always: // If breakpoint is Always, return the reverse direction, honestly the user should just use the Row Property
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{reverseState}", "gap-3" });
                    break;
                case Breakpoint.Xs: // Xs is Reverse Direction, Sm and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{reverseState}", $"flex-sm-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.Sm: // Xs is Default Direction, Sm is Reverse Direction, Md and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-sm-{reverseState}", $"flex-md-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.Md: // Xs to Sm is Default Direction, Md is Reverse Direction, Lg and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-md-{reverseState}", $"flex-lg-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.Lg: // Xs to Md is Default Direction, Lg is Reverse Direction, Xl and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-lg-{reverseState}", $"flex-xl-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.Xl: // Xs to Lg is Default Direction, Xl is Reverse Direction, Xxl is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-xl-{reverseState}", $"flex-xxl-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.Xxl: // Xs to Xl is Default Direction, Xxl is Reverse Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-xxl-{reverseState}", "gap-3" });
                    break;
                case Breakpoint.SmAndDown: // Sm and Down is Reverse Direction, Md and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{reverseState}", $"flex-md-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.MdAndDown: // Md and Down is Reverse Direction, Lg and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{reverseState}", $"flex-lg-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.LgAndDown: // Lg and Down is Reverse Direction, Xl and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{reverseState}", $"flex-xl-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.XlAndDown: // Xl and Down is Reverse Direction, Xxl and Up is Default Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{reverseState}", $"flex-xxl-{defaultState}", "gap-3" });
                    break;
                case Breakpoint.SmAndUp: // Xs is Default Direction, Sm and Up is Reverse Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-sm-{reverseState}", "gap-3" });
                    break;
                case Breakpoint.MdAndUp: // Xs to Sm is Default Direction, Md and Up is Reverse Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-md-{reverseState}", "gap-3" });
                    break;
                case Breakpoint.LgAndUp: // Xs to Md is Default Direction, Lg and Up is Reverse Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-lg-{reverseState}", "gap-3" });
                    break;
                case Breakpoint.XlAndUp: // Xs to Lg is Default Direction, Xl and Up is Reverse Direction
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", $"flex-xl-{reverseState}", "gap-3" });
                    break;
                default: // Return the default direction if no Breakpoint is Matched
                    stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", $"flex-{defaultState}", "gap-3" });
                    break;
            }
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



