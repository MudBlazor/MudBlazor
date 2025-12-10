using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.TestComponents.Button;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ButtonsTests : BunitTest
    {
        /// <summary>
        /// MudButton without specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudButtonShouldRenderAButtonByDefault()
        {
            var comp = Context.Render<MudButton>();
            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");
            //it is a button, and has by default stopPropagation on onclick
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button")
                .And
                .Contain("stopPropagation");
        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudButtonShouldRenderAnAnchorIfLinkIsSetAndIsNotDisabled()
        {
            var comp = Context.Render<MudButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank"));
            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");
            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");
            //it is an anchor and not contains stopPropagation 
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a")
                .And
                .NotContain("__internal_stopPropagation_onclick");

            comp = Context.Render<MudButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank")
                .Add(p => p.Disabled, true));
            comp.Instance.HtmlTag.Should().Be("button");

        }

        /// <summary>
        /// MudButton should render with value of Rel property
        /// </summary>
        [Test]
        public void MudButtonShouldRenderRelIfSet()
        {
            var comp = Context.Render<MudButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, "nofollow"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("nofollow");
        }

        /// <summary>
        /// MudButton should have rel="nofollow" if Rel is set to "nofollow", even if Target is _blank
        /// </summary>
        [Test]
        public void MudButtonShouldHaveNoopenerOverridenByRel()
        {
            var comp = Context.Render<MudButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank")
                .Add(p => p.Rel, "nofollow"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("nofollow");
        }

        /// <summary>
        /// MudButton should have rel="" Rel is explicitly set to empty, even if Target is _blank
        /// </summary>
        [Test]
        public void MudButtonShouldHaveHaveNoRelWhenSetToEmpty()
        {
            var comp = Context.Render<MudButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, "")
                .Add(p => p.Target, "_blank"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("");
        }

        /// <summary>
        /// MudButton should not render rel if it's null and target is not _blank
        /// </summary>
        [Test]
        public void MudButtonShouldNotRenderRelIfNullAndTargetNotBlank()
        {
            var comp = Context.Render<MudButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, null)
                .Add(p => p.Target, "_notblank"));
            comp
                .Find("a")
                .HasAttribute("rel")
                .Should()
                .BeFalse();
        }

        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderAButtonByDefault()
        {
            var comp = Context.Render<MudIconButton>();
            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");
            //it is a button
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button");
        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderAnAnchorIfLinkIsSet()
        {
            using var ctx = new Bunit.BunitContext();
            var comp = ctx.Render<MudIconButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank"));
            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");
            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");
            //it is an anchor
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a");
        }

        /// <summary>
        /// MudIconButton should render with value of Rel property
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderRelIfSet()
        {
            var comp = Context.Render<MudIconButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, "nofollow"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("nofollow");
        }

        /// <summary>
        /// MudIconButton should have rel="nofollow" if Rel is set to "nofollow", even if Target is _blank
        /// </summary>
        [Test]
        public void MudIconButtonShouldHaveNoopenerOverridenByRel()
        {
            var comp = Context.Render<MudIconButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank")
                .Add(p => p.Rel, "nofollow"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("nofollow");
        }

        /// <summary>
        /// MudButton should have rel="" Rel is explicitly set to empty, even if Target is _blank
        /// </summary>
        [Test]
        public void MudIconButtonShouldHaveHaveNoRelWhenSetToEmpty()
        {
            var comp = Context.Render<MudIconButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank")
                .Add(p => p.Rel, ""));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("");
        }

        /// <summary>
        /// MudIconButton should not render rel if it's null and target is not _blank
        /// </summary>
        [Test]
        public void MudIconButtonShouldNotRenderRelIfNullAndTargetNotBlank()
        {
            var comp = Context.Render<MudIconButton>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, null)
                .Add(p => p.Target, "_notblank"));
            comp
                .Find("a")
                .HasAttribute("rel")
                .Should()
                .BeFalse();
        }

        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudFabShouldRenderAButtonByDefault()
        {
            var comp = Context.Render<MudFab>();
            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");
            //it is a button
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button");
        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudFabShouldRenderAnAnchorIfLinkIsSet()
        {
            var comp = Context.Render<MudFab>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank"));
            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");
            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");
            //it is an anchor
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a");
        }

        /// <summary>
        /// MudFab should only render an icon if one is specified.
        /// </summary>
        [Test]
        public void MudFabShouldNotRenderIconIfNoneSpecified()
        {
            var comp = Context.Render<MudFab>();
            comp.Markup
                .Should()
                .NotContainAny("mud-icon-root");
        }

        /// <summary>
        /// MudFab should render with value of Rel property
        /// </summary>
        [Test]
        public void MudFabShouldRenderRelIfSet()
        {
            var comp = Context.Render<MudFab>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, "nofollow"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("nofollow");
        }

        /// <summary>
        /// MudFab should have rel="nofollow" if Rel is set to "nofollow", even if Target is _blank
        /// </summary>
        [Test]
        public void MudFabShouldHaveNoopenerOverridenByRel()
        {
            var comp = Context.Render<MudFab>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank")
                .Add(p => p.Rel, "nofollow"));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("nofollow");
        }

        /// <summary>
        /// MudFab should have rel="" Rel is explicitly set to empty, even if Target is _blank
        /// </summary>
        [Test]
        public void MudFabShouldHaveHaveNoRelWhenSetToEmpty()
        {
            var comp = Context.Render<MudFab>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Target, "_blank")
                .Add(p => p.Rel, ""));
            comp
                .Find("a")
                .GetAttribute("rel")
                .Should()
                .Be("");
        }

        /// <summary>
        /// MudFab should not render rel if it's null and target is not _blank
        /// </summary>
        [Test]
        public void MudFabShouldNotRenderRelIfNullAndTargetNotBlank()
        {
            var comp = Context.Render<MudFab>(parameters => parameters
                .Add(p => p.Href, "https://www.google.com")
                .Add(p => p.Rel, null)
                .Add(p => p.Target, "_notblank"));
            comp
                .Find("a")
                .HasAttribute("rel")
                .Should()
                .BeFalse();
        }

        [Test]
        public async Task MudToggleIconTest()
        {
            var comp = Context.Render<MudToggleIconButton>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => comp.Instance.SetToggledAsync(true));
            await comp.WaitForAssertionAsync(() => comp.Instance.Toggled.Should().BeFalse());
        }

        [Test]
        public void MudButtonSizesTest()
        {
            var comp = Context.Render<ButtonSizeIconSizeTest>();

            var buttons = comp.Nodes.Where(n => n.NodeName.Equals("BUTTON")).ToArray();
            buttons.Length.Should().Be(6);

            // Buttons 1-3: Explicit button sizes
            ((IHtmlButtonElement)buttons[0]).ClassList.Contains("mud-button-filled-size-small").Should().BeTrue();  // Size="Size.Small"
            ((IHtmlButtonElement)buttons[1]).ClassList.Contains("mud-button-filled-size-medium").Should().BeTrue(); // Size="Size.Medium"
            ((IHtmlButtonElement)buttons[2]).ClassList.Contains("mud-button-filled-size-large").Should().BeTrue();  // Size="Size.Large"
        }

        [Test]
        public void MudButtonIconSizesTest()
        {
            var comp = Context.Render<ButtonSizeIconSizeTest>();

            var buttons = comp.Nodes.Where(n => n.NodeName.Equals("BUTTON")).ToArray();

            // Button 4: Small button- with large icon size: Size="Size.Small", IconSize="Size.Large"
            ((IHtmlButtonElement)buttons[3]).ClassList.Contains("mud-button-filled-size-small").Should().BeTrue();
            var button4Span = ((IHtmlButtonElement)buttons[3]).Children[0].Children[0];
            button4Span.ClassName.Contains("mud-button-icon-size-large").Should().BeTrue();
            var button4Svg = button4Span.Children[0];
            button4Svg.ClassName.Contains("mud-icon-size-large").Should().BeTrue();

            // Button 5: Defaults: Medium button- and icon size.
            ((IHtmlButtonElement)buttons[4]).ClassList.Contains("mud-button-filled-size-medium").Should().BeTrue();
            var button5Span = ((IHtmlButtonElement)buttons[4]).Children[0].Children[0];
            button5Span.ClassName.Contains("mud-button-icon-size-medium").Should().BeTrue();
            var button5Svg = button5Span.Children[0];
            button5Svg.ClassName.Contains("mud-icon-size-medium").Should().BeTrue();

            // Button 6: Large button- with small icon size: Size="Size.Large", IconSize="Size.Small"
            ((IHtmlButtonElement)buttons[5]).ClassList.Contains("mud-button-filled-size-large").Should().BeTrue();
            var button6Span = ((IHtmlButtonElement)buttons[5]).Children[0].Children[0];
            button6Span.ClassName.Contains("mud-button-icon-size-small").Should().BeTrue();
            var button6Svg = button6Span.Children[0];
            button6Svg.ClassName.Contains("mud-icon-size-small").Should().BeTrue();
        }

        /// <summary>
        /// Ensures buttons inherit their disabled state
        /// </summary>
        [Test]
        public async Task ButtonsNestedDisabledTest()
        {
            var comp = Context.Render<ButtonsNestedDisabledTest>();

            comp.FindComponent<MudButton>().Find("button").HasAttribute("disabled").Should().BeFalse();
            comp.FindComponent<MudFab>().Find("button").HasAttribute("disabled").Should().BeFalse();
            comp.FindComponent<MudIconButton>().Find("button").HasAttribute("disabled").Should().BeFalse();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true)); //buttons should be disabled when the cascading value is disabled

            comp.FindComponent<MudButton>().Find("button").HasAttribute("disabled").Should().BeTrue();
            comp.FindComponent<MudFab>().Find("button").HasAttribute("disabled").Should().BeTrue();
            comp.FindComponent<MudIconButton>().Find("button").HasAttribute("disabled").Should().BeTrue();
        }

        [Test]
        public async Task ButtonsOnClickErrorContentCaughtException()
        {
            var comp = Context.Render<ButtonErrorContenCaughtException>();
            var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
            IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
            IReadOnlyList<IElement> Buttons() => comp.FindAll("button.mud-button-root");
            IElement MudButton() => Buttons()[0];
            IElement MudFab() => Buttons()[1];
            IElement MudIconButton() => Buttons()[2];

            // MudButton
            await MudButton().ClickAsync(new MouseEventArgs());
            alertTextFunc().InnerHtml.Should().Be("Something went wrong...");
            await comp.InvokeAsync(comp.Instance.Recover);
            alertTextFunc.Should().Throw<ComponentNotFoundException>();

            // MudFab
            await MudFab().ClickAsync(new MouseEventArgs());
            alertTextFunc().InnerHtml.Should().Be("Something went wrong...");
            await comp.InvokeAsync(comp.Instance.Recover);
            alertTextFunc.Should().Throw<ComponentNotFoundException>();

            // MudIconButton
            await MudIconButton().ClickAsync(new MouseEventArgs());
            alertTextFunc().InnerHtml.Should().Be("Something went wrong...");
            await comp.InvokeAsync(comp.Instance.Recover);
            alertTextFunc.Should().Throw<ComponentNotFoundException>();
        }
    }
}
