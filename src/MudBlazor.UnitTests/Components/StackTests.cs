using AwesomeAssertions;
using Bunit;

namespace MudBlazor.UnitTests.Components
{
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
            stack.HtmlTag.Should().Be("div");
        }

        [Test]
        public void CheckDefaultClass()
        {
            var stack = Context.Render<MudStack>();

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", "gap-3" });
        }

        [Test]
        public void CheckRowClass()
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.Row, true));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-row", "gap-3" });
        }

        [Test]
        public void CheckReverseClass()
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.Reverse, true));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column-reverse", "gap-3" });
        }

        [Test]
        [Arguments(0)]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(4)]
        [Arguments(5)]
        [Arguments(6)]
        [Arguments(7)]
        [Arguments(8)]
        [Arguments(9)]
        [Arguments(10)]
        [Arguments(11)]
        [Arguments(12)]
        [Arguments(13)]
        [Arguments(14)]
        [Arguments(15)]
        [Arguments(16)]
        public void CheckSpacingClass(int spacing)
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.Spacing, spacing));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"gap-{spacing}" });
        }

        [Test]
        [Arguments(Breakpoint.None)]
        [Arguments(Breakpoint.Always)]
        [Arguments(Breakpoint.Xs)]
        [Arguments(Breakpoint.Sm)]
        [Arguments(Breakpoint.Md)]
        [Arguments(Breakpoint.Lg)]
        [Arguments(Breakpoint.Xl)]
        [Arguments(Breakpoint.Xxl)]
        [Arguments(Breakpoint.SmAndDown)]
        [Arguments(Breakpoint.MdAndDown)]
        [Arguments(Breakpoint.LgAndDown)]
        [Arguments(Breakpoint.XlAndDown)]
        [Arguments(Breakpoint.SmAndUp)]
        [Arguments(Breakpoint.MdAndUp)]
        [Arguments(Breakpoint.LgAndUp)]
        [Arguments(Breakpoint.XlAndUp)]
        [Arguments(Breakpoint.None, true)]
        [Arguments(Breakpoint.Always, true)]
        [Arguments(Breakpoint.Xs, true)]
        [Arguments(Breakpoint.Sm, true)]
        [Arguments(Breakpoint.Md, true)]
        [Arguments(Breakpoint.Lg, true)]
        [Arguments(Breakpoint.Xl, true)]
        [Arguments(Breakpoint.Xxl, true)]
        [Arguments(Breakpoint.SmAndDown, true)]
        [Arguments(Breakpoint.MdAndDown, true)]
        [Arguments(Breakpoint.LgAndDown, true)]
        [Arguments(Breakpoint.XlAndDown, true)]
        [Arguments(Breakpoint.SmAndUp, true)]
        [Arguments(Breakpoint.MdAndUp, true)]
        [Arguments(Breakpoint.LgAndUp, true)]
        [Arguments(Breakpoint.XlAndUp, true)]
        [Arguments(Breakpoint.None, true, true)]
        [Arguments(Breakpoint.Always, true, true)]
        [Arguments(Breakpoint.Xs, true, true)]
        [Arguments(Breakpoint.Sm, true, true)]
        [Arguments(Breakpoint.Md, true, true)]
        [Arguments(Breakpoint.Lg, true, true)]
        [Arguments(Breakpoint.Xl, true, true)]
        [Arguments(Breakpoint.Xxl, true, true)]
        [Arguments(Breakpoint.SmAndDown, true, true)]
        [Arguments(Breakpoint.MdAndDown, true, true)]
        [Arguments(Breakpoint.LgAndDown, true, true)]
        [Arguments(Breakpoint.XlAndDown, true, true)]
        [Arguments(Breakpoint.SmAndUp, true, true)]
        [Arguments(Breakpoint.MdAndUp, true, true)]
        [Arguments(Breakpoint.LgAndUp, true, true)]
        [Arguments(Breakpoint.XlAndUp, true, true)]
        public void CheckBreakpointClass(Breakpoint breakpoint, bool row = false, bool reverse = false)
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.Breakpoint, breakpoint).Add(c => c.Row, row).Add(c => c.Reverse, reverse));

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
        [Arguments(Justify.FlexStart, "start")]
        [Arguments(Justify.Center, "center")]
        [Arguments(Justify.FlexEnd, "end")]
        [Arguments(Justify.SpaceBetween, "space-between")]
        [Arguments(Justify.SpaceAround, "space-around")]
        [Arguments(Justify.SpaceEvenly, "space-evenly")]
        public void CheckJustifyClass(Justify justify, string expectedClass)
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.Justify, justify));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"justify-{expectedClass}", "gap-3" });
        }

        [Test]
        [Arguments(AlignItems.Baseline, "baseline")]
        [Arguments(AlignItems.Center, "center")]
        [Arguments(AlignItems.Start, "start")]
        [Arguments(AlignItems.End, "end")]
        [Arguments(AlignItems.Stretch, "stretch")]
        public void CheckAlignItemsClass(AlignItems align, string expectedClass)
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.AlignItems, align));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"align-{expectedClass}", "gap-3" });
        }

        [Test]
        [Arguments(StretchItems.Start, "start")]
        [Arguments(StretchItems.End, "end")]
        [Arguments(StretchItems.StartAndEnd, "start-and-end")]
        [Arguments(StretchItems.Middle, "middle")]
        [Arguments(StretchItems.All, "all")]
        public void CheckStretchItemsClass(StretchItems stretch, string expectedClass)
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.StretchItems, stretch));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().Contain(["d-flex", $"flex-grow-{expectedClass}"]);
        }

        [Test]
        public void CheckStretchItemsNoneClass()
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.StretchItems, StretchItems.None));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().NotContain(["flex-grow-start", "flex-grow-end", "flex-grow-start-and-end", "flex-grow-all"]);
        }

        [Test]
        [Arguments(Wrap.NoWrap, "nowrap")]
        [Arguments(Wrap.Wrap, "wrap")]
        [Arguments(Wrap.WrapReverse, "wrap-reverse")]
        public void CheckWrapClass(Wrap wrap, string expectedClass)
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.Wrap, wrap));

            var stackClass = stack.Find(".d-flex");
            stackClass.ClassList.Should().ContainInOrder(new[] { "d-flex", "flex-column", $"flex-{expectedClass}", "gap-3" });
        }

        [Test]
        public void HtmlTagSetToUlRendersUlElement()
        {
            var stack = Context.Render<MudStack>(x => x.Add(c => c.HtmlTag, "ul"));
            var stackElement = stack.Find("ul.d-flex");

            stackElement.Should().NotBeNull();
            stackElement.HasAttribute("role").Should().BeFalse();
        }

        [Test]
        public void DefaultStackRendersDivElementWithGroupRole()
        {
            var stack = Context.Render<MudStack>();
            var stackElement = stack.Find("div.d-flex");

            stackElement.Should().NotBeNull();
            stackElement.GetAttribute("role").Should().Be("group");
        }

        [Test]
        public void UserSuppliedRoleOverridesDefaultRoleForDiv()
        {
            var divStack = Context.Render<MudStack>(parameters => parameters
                .AddUnmatched("role", "list"));
            var divElement = divStack.Find("div.d-flex");

            divElement.GetAttribute("role").Should().Be("list");
        }

        [Test]
        public void SemanticTagDoesNotRenderRoleAttribute()
        {
            var semanticStack = Context.Render<MudStack>(parameters => parameters
                .Add(c => c.HtmlTag, "ul"));
            var semanticElement = semanticStack.Find("ul.d-flex");

            semanticElement.HasAttribute("role").Should().BeFalse();
        }
    }
}