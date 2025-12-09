using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Link;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class LinkTests : BunitTest
{
    [Test]
    public void DefaultPropertyValues()
    {
        var comp = Context.RenderComponent<MudLink>();

        comp.Instance.Color.Should().Be(Color.Primary);
        comp.Instance.Typo.Should().Be(Typo.inherit);
        comp.Instance.Underline.Should().Be(Underline.Hover);
        comp.Instance.Href.Should().BeNull();
        comp.Instance.Target.Should().BeNull();
        comp.Instance.Disabled.Should().BeFalse();
    }

    [Test]
    public void DefaultTypo_ShouldInheritParentTypography()
    {
        var comp = Context.RenderComponent<MudLink>();

        var linkElement = comp.Find("a");
        linkElement.GetAttribute("class").Should().NotContain("mud-typography-");
    }

    [Test]
    public void InlineLink_ShouldRenderWithParentTypography()
    {
        var comp = Context.RenderComponent<MudText>(parameters => parameters
            .Add(p => p.Typo, Typo.caption)
            .AddChildContent(childBuilder =>
            {
                childBuilder.AddContent(0, "MudBlazor is ");
                childBuilder.OpenComponent<MudLink>(1);
                childBuilder.AddAttribute(2, nameof(MudLink.ChildContent), (RenderFragment)(linkBuilder => linkBuilder.AddContent(0, "Awesome")));
                childBuilder.CloseComponent();
            }));

        var textElement = comp.Find("span.mud-typography");
        textElement.ClassList.Should().Contain("mud-typography-caption");

        var linkElement = textElement.QuerySelector("a.mud-link")!;
        linkElement.ClassList.Should().Contain("mud-typography");
        linkElement.GetAttribute("class").Should().NotContain("mud-typography-");
        linkElement.TextContent.Should().Be("Awesome");

        comp.MarkupMatches("""<span class="mud-typography mud-typography-caption">MudBlazor is <a class="mud-typography mud-link mud-primary-text mud-link-underline-hover">Awesome</a></span>""");
    }

    [Test]
    public void DisabledProperty_DisplaysAsDisabled()
    {
        var comp = Context.RenderComponent<MudLink>(parameters => parameters
            .Add(x => x.Href, "#")
            .Add(x => x.Disabled, true));

        var linkElement = comp.Find("a");
        linkElement.GetAttribute("href").Should().BeNullOrEmpty();
        linkElement.GetAttribute("aria-disabled").Should().Be("true");
        linkElement.ClassList.Should().Contain("mud-link-disabled");
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ShouldExecute_OnClick(bool disabled)
    {
        var calls = 0;
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .Add(p => p.OnClick, e => calls++)
            .Add(p => p.Disabled, disabled)
        );

        await comp.Find("a").ClickAsync(new MouseEventArgs());

        if (disabled)
        {
            calls.Should().Be(0);
        }
        else
        {
            calls.Should().Be(1);
        }
    }

    [Test]
    public async Task OnClickErrorContentCaughtException()
    {
        var comp = Context.RenderComponent<LinkErrorContenCaughtException>();
        IElement AlertText() => MudAlert().Find("div.mud-alert-message");
        IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
        IRefreshableElementCollection<IElement> Links() => comp.FindAll("a.mud-link");
        IElement MudLink() => Links()[0];

        await MudLink().ClickAsync(new MouseEventArgs());

        AlertText().InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
    }

    [TestCase(Color.Primary, "mud-primary-text")]
    [TestCase(Color.Secondary, "mud-secondary-text")]
    [TestCase(Color.Tertiary, "mud-tertiary-text")]
    public void ColorProperty_AppliesCorrectClass(Color color, string expectedClass)
    {
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .Add(p => p.Color, color)
        );

        var linkElement = comp.Find("a");
        linkElement.ClassList.Should().Contain(expectedClass);
    }

    [TestCase(Typo.h1, "mud-typography-h1")]
    [TestCase(Typo.subtitle1, "mud-typography-subtitle1")]
    [TestCase(Typo.caption, "mud-typography-caption")]
    public void TypoProperty_AppliesCorrectClass(Typo typo, string expectedClass)
    {
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .Add(p => p.Typo, typo)
        );

        var linkElement = comp.Find("a");
        linkElement.ClassList.Should().Contain(expectedClass);
    }

    [TestCase(Underline.None, "mud-link-underline-none")]
    [TestCase(Underline.Hover, "mud-link-underline-hover")]
    [TestCase(Underline.Always, "mud-link-underline-always")]
    public void UnderlineProperty_AppliesCorrectClass(Underline underline, string expectedClass)
    {
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .Add(p => p.Underline, underline)
        );

        var linkElement = comp.Find("a");
        linkElement.ClassList.Should().Contain(expectedClass);
    }

    [TestCase("_blank")]
    [TestCase("_self")]
    [TestCase("_parent")]
    [TestCase("_top")]
    public void TargetProperty_AppliesCorrectAttribute(string target)
    {
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .Add(p => p.Href, "#")
            .Add(p => p.Target, target)
        );

        var linkElement = comp.Find("a");
        linkElement.GetAttribute("target").Should().Be(target);
    }

    [Test]
    public void ChildContent_IsRenderedCorrectly()
    {
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .AddChildContent("<span>Test content</span>")
        );

        var linkElement = comp.Find("a");
        linkElement.InnerHtml.Should().Be("<span>Test content</span>");
    }

    [Test]
    public void UserAttributes_OverrideDefaultAttributes()
    {
        var comp = Context.RenderComponent<MudLink>(builder => builder
            .Add(p => p.Href, "#")
            .Add(p => p.Target, "_self")
            .Add(p => p.UserAttributes, new()
            {
                { "target", "custom-target" },
                { "role", "custom-role" },
                { "custom-attribute", "custom-value" }
            })
        );

        var linkElement = comp.Find("a");

        // Verify that user-defined attributes override default attributes, even ones set by properties.
        linkElement.GetAttribute("target").Should().Be("custom-target");
        linkElement.GetAttribute("role").Should().Be("custom-role");
        linkElement.GetAttribute("custom-attribute").Should().Be("custom-value");
    }
}
