using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

/// <summary>
/// Covers the behavior every button inherits from <see cref="MudBaseButton"/>: the root element, links, disabling, clicks, and attribute precedence.
/// </summary>
/// <remarks>
/// Each test runs once per button type, so a regression in the shared base shows up against every component that renders through it.
/// </remarks>
[TestFixture(typeof(MudButton))]
[TestFixture(typeof(MudIconButton))]
[TestFixture(typeof(MudFab))]
public class BaseButtonTests<TButton> : BunitTest where TButton : MudBaseButton
{
    /// <summary>
    /// Without an Href, the root is an enabled button element of type button, so it never submits a form by accident.
    /// </summary>
    [Test]
    public void RendersButtonElementByDefault()
    {
        var comp = Context.Render<TButton>();

        var root = comp.Find(".mud-button-root");
        root.TagName.Should().Be("BUTTON");
        root.GetAttribute("type").Should().Be("button");
        root.HasAttribute("disabled").Should().BeFalse();
        root.HasAttribute("href").Should().BeFalse();
    }

    /// <summary>
    /// ButtonType sets the type attribute of the button element.
    /// </summary>
    [TestCase(ButtonType.Submit, "submit")]
    [TestCase(ButtonType.Reset, "reset")]
    public void ButtonTypeSetsTypeAttribute(ButtonType buttonType, string expected)
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.ButtonType, buttonType));

        comp.Find("button").GetAttribute("type").Should().Be(expected);
    }

    /// <summary>
    /// Setting Href renders an anchor that carries the link attributes.
    /// It has no type attribute, which on an anchor is a MIME type hint for the linked resource.
    /// </summary>
    [Test]
    public void HrefRendersAnchor()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .Add(p => p.Target, "_self"));

        var root = comp.Find(".mud-button-root");
        root.TagName.Should().Be("A");
        root.GetAttribute("href").Should().Be("/docs");
        root.GetAttribute("target").Should().Be("_self");
        root.HasAttribute("type").Should().BeFalse();
    }

    /// <summary>
    /// A link that opens a new tab gets rel="noopener" unless Rel is set, and an explicit Rel, even an empty one, is rendered as given.
    /// </summary>
    [TestCase("_blank", null, "noopener")]
    [TestCase("_blank", "nofollow", "nofollow")]
    [TestCase("_blank", "", "")]
    [TestCase("_self", null, null)]
    [TestCase(null, "nofollow", "nofollow")]
    public void RelDefaultsToNoopenerOnlyForBlankTarget(string target, string rel, string expectedRel)
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Target, target)
            .Add(p => p.Rel, rel));

        comp.Find("a").GetAttribute("rel").Should().Be(expectedRel);
    }

    /// <summary>
    /// A custom HtmlTag is rendered as given, which is how a button becomes the label of a file input.
    /// </summary>
    [Test]
    public void HtmlTagRendersThatElement()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.HtmlTag, "label"));

        comp.Find(".mud-button-root").TagName.Should().Be("LABEL");
    }

    /// <summary>
    /// A disabled link falls back to a disabled button and drops its link attributes, so it cannot be followed.
    /// </summary>
    [Test]
    public void DisabledLinkRendersDisabledButton()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .Add(p => p.Target, "_blank")
            .Add(p => p.Disabled, true));

        var root = comp.Find(".mud-button-root");
        root.TagName.Should().Be("BUTTON");
        root.HasAttribute("disabled").Should().BeTrue();
        root.HasAttribute("href").Should().BeFalse();
        root.HasAttribute("target").Should().BeFalse();
        root.HasAttribute("rel").Should().BeFalse();
    }

    /// <summary>
    /// A disabled link drops an explicit Rel along with its other link attributes.
    /// </summary>
    [Test]
    public void DisabledLinkDropsExplicitRel()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .Add(p => p.Rel, "nofollow")
            .Add(p => p.Disabled, true));

        comp.Find("button").HasAttribute("rel").Should().BeFalse();
    }

    /// <summary>
    /// Clearing Href on a re-render turns the anchor back into a button, which keyboard users can focus and activate.
    /// </summary>
    [Test]
    public async Task ClearingHrefRendersButtonAgain()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "/docs"));

        comp.Find(".mud-button-root").TagName.Should().Be("A");

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Href, (string)null));

        var root = comp.Find(".mud-button-root");
        root.TagName.Should().Be("BUTTON");
        root.GetAttribute("type").Should().Be("button");
        root.HasAttribute("href").Should().BeFalse();
    }

    /// <summary>
    /// Enabling a disabled link restores the anchor with its href, target and rel.
    /// </summary>
    [Test]
    public async Task EnablingDisabledLinkRestoresAnchor()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .Add(p => p.Target, "_blank")
            .Add(p => p.Disabled, true));

        comp.Find(".mud-button-root").TagName.Should().Be("BUTTON");

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Disabled, false));

        var root = comp.Find(".mud-button-root");
        root.TagName.Should().Be("A");
        root.HasAttribute("disabled").Should().BeFalse();
        root.GetAttribute("href").Should().Be("/docs");
        root.GetAttribute("target").Should().Be("_blank");
        root.GetAttribute("rel").Should().Be("noopener");
    }

    /// <summary>
    /// Clearing Href restores a custom HtmlTag that the link replaced.
    /// </summary>
    [Test]
    public async Task ClearingHrefRestoresHtmlTag()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.HtmlTag, "label")
            .Add(p => p.Href, "/docs"));

        comp.Find(".mud-button-root").TagName.Should().Be("A");

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Href, (string)null));

        comp.Find(".mud-button-root").TagName.Should().Be("LABEL");
    }

    /// <summary>
    /// Rendering an enabled or disabled link leaves HtmlTag, Href and Target as the caller supplied them.
    /// </summary>
    [Test]
    public async Task LinkParametersAreNotOverwritten()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .Add(p => p.Target, "_blank"));

        comp.Instance.HtmlTag.Should().Be("button");

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Disabled, true));

        comp.Instance.Href.Should().Be("/docs");
        comp.Instance.Target.Should().Be("_blank");
    }

    /// <summary>
    /// Clicking an enabled button invokes OnClick once.
    /// </summary>
    [Test]
    public async Task ClickInvokesOnClick()
    {
        var clicks = 0;
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.OnClick, () => clicks++));

        await comp.Find("button").ClickAsync();

        clicks.Should().Be(1);
    }

    /// <summary>
    /// Clicking a disabled button does not invoke OnClick.
    /// </summary>
    [Test]
    public async Task DisabledClickDoesNotInvokeOnClick()
    {
        var clicks = 0;
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, () => clicks++));

        await comp.Find("button").ClickAsync();

        clicks.Should().Be(0);
    }

    /// <summary>
    /// A disabled parent, such as a disabled form, disables the button the same way its own Disabled parameter does.
    /// </summary>
    [Test]
    public async Task ParentDisabledDisablesButton()
    {
        var clicks = 0;
        var comp = Context.Render<CascadingValue<bool>>(parameters => parameters
            .Add(p => p.Name, "ParentDisabled")
            .Add(p => p.Value, true)
            .AddChildContent<TButton>(button => button
                .Add(p => p.Href, "/docs")
                .Add(p => p.OnClick, () => clicks++)));

        var root = comp.Find(".mud-button-root");
        root.TagName.Should().Be("BUTTON");
        root.HasAttribute("disabled").Should().BeTrue();
        root.HasAttribute("href").Should().BeFalse();

        await comp.Find("button").ClickAsync();
        clicks.Should().Be(0);
    }

    /// <summary>
    /// A click activates the cascaded <see cref="IActivatable"/> after OnClick, which is how menus and other hosts react to their activator.
    /// </summary>
    [Test]
    public async Task ClickActivatesCascadedActivatableAfterOnClick()
    {
        var events = new List<string>();
        var activatable = new RecordingActivatable(events);
        var comp = Context.Render<CascadingValue<IActivatable>>(parameters => parameters
            .Add(p => p.Value, activatable)
            .AddChildContent<TButton>(button => button
                .Add(p => p.OnClick, () => events.Add("click"))));

        await comp.Find("button").ClickAsync();

        events.Should().Equal("click", "activate");
        activatable.Activator.Should().BeSameAs(comp.FindComponent<TButton>().Instance);
    }

    /// <summary>
    /// A disabled button does not activate the cascaded <see cref="IActivatable"/>.
    /// </summary>
    [Test]
    public async Task DisabledClickDoesNotActivateCascadedActivatable()
    {
        var events = new List<string>();
        var comp = Context.Render<CascadingValue<IActivatable>>(parameters => parameters
            .Add(p => p.Value, new RecordingActivatable(events))
            .AddChildContent<TButton>(button => button
                .Add(p => p.Disabled, true)));

        await comp.Find("button").ClickAsync();

        events.Should().BeEmpty();
    }

    /// <summary>
    /// A button stops its click from reaching parent handlers unless ClickPropagation is set, while links and other tags always let it through.
    /// </summary>
    [TestCase(null, null, false, 0)]
    [TestCase(null, null, true, 1)]
    [TestCase("/docs", null, false, 1)]
    [TestCase(null, "label", false, 1)]
    public async Task ClickPropagatesOnlyWhenAllowed(string href, string htmlTag, bool clickPropagation, int expectedParentClicks)
    {
        var parentClicks = 0;
        var comp = Context.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => parentClicks++));
            builder.OpenComponent<TButton>(2);
            builder.AddComponentParameter(3, nameof(MudBaseButton.Href), href);
            builder.AddComponentParameter(4, nameof(MudBaseButton.ClickPropagation), clickPropagation);
            if (htmlTag is not null)
            {
                builder.AddComponentParameter(5, nameof(MudBaseButton.HtmlTag), htmlTag);
            }
            builder.CloseComponent();
            builder.CloseElement();
        });

        await comp.Find(".mud-button-root").ClickAsync();

        parentClicks.Should().Be(expectedParentClicks);
    }

    /// <summary>
    /// An exception thrown by an async OnClick handler reaches the surrounding ErrorBoundary instead of being swallowed.
    /// </summary>
    [Test]
    public async Task OnClickExceptionReachesErrorBoundary()
    {
        var comp = Context.Render<ErrorBoundary>(parameters => parameters
            .AddChildContent<TButton>(button => button
                .Add(p => p.OnClick, async () =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("Something went wrong");
                }))
            .Add(p => p.ErrorContent, exception => $"<p class=\"error\">{exception.Message}</p>"));

        await comp.Find("button").ClickAsync();

        comp.Find("p.error").TextContent.Should().Be("Something went wrong");
        comp.FindAll("button").Should().BeEmpty();
    }

    /// <summary>
    /// FocusAsync focuses the root element, which requires the element reference to be captured.
    /// </summary>
    [Test]
    public async Task FocusAsyncFocusesRootElement()
    {
        var comp = Context.Render<TButton>();

        await comp.InvokeAsync(() => comp.Instance.FocusAsync().AsTask());

        var invocation = Context.JSInterop.VerifyFocusAsyncInvoke();
        var focused = (ElementReference)invocation.Arguments[0];
        focused.Id.Should().Be(comp.Find("button").GetAttribute("blazor:elementReference"));
    }

    /// <summary>
    /// Capturing the element reference must not cost a button a second render (#13519).
    /// </summary>
    [Test]
    public void RendersOnce()
    {
        Context.Render<TButton>().RenderCount.Should().Be(1);
    }

    /// <summary>
    /// Class and Style are applied to the root element alongside the component's own classes.
    /// </summary>
    [Test]
    public void ClassAndStyleApplyToRoot()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Class, "my-button")
            .Add(p => p.Style, "color:blue"));

        var root = comp.Find(".mud-button-root");
        root.ClassList.Should().Contain("my-button");
        root.GetAttribute("style").Should().Be("color:blue");
    }

    /// <summary>
    /// A class or style supplied through UserAttributes replaces the computed ones, for buttons and links alike.
    /// </summary>
    [TestCase(null, "BUTTON")]
    [TestCase("/docs", "A")]
    public void UserAttributesOverrideComputedClassAndStyle(string href, string expectedTag)
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.Href, href)
            .Add(p => p.Style, "color:blue")
            .Add(p => p.UserAttributes, new Dictionary<string, object>
            {
                ["class"] = "user-class",
                ["style"] = "color:red",
                ["aria-label"] = "Save",
            }));

        var root = comp.Find(".user-class");
        root.TagName.Should().Be(expectedTag);
        root.GetAttribute("class").Should().Be("user-class");
        root.GetAttribute("style").Should().Be("color:red");
        root.GetAttribute("aria-label").Should().Be("Save");
    }

    /// <summary>
    /// Attributes a button derives from its own parameters win over the same names supplied through UserAttributes.
    /// </summary>
    [Test]
    public void ParameterAttributesWinOverUserAttributes()
    {
        var comp = Context.Render<TButton>(parameters => parameters
            .Add(p => p.ButtonType, ButtonType.Submit)
            .Add(p => p.Disabled, true)
            .Add(p => p.UserAttributes, new Dictionary<string, object>
            {
                ["type"] = "reset",
                ["disabled"] = false,
            }));

        var root = comp.Find("button");
        root.GetAttribute("type").Should().Be("submit");
        root.HasAttribute("disabled").Should().BeTrue();
    }

    private sealed class RecordingActivatable(List<string> events) : IActivatable
    {
        public object Activator { get; private set; }

        public void Activate(object activator, MouseEventArgs args)
        {
            Activator = activator;
            events.Add("activate");
        }
    }
}
