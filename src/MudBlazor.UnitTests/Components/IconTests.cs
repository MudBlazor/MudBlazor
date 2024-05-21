using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class IconTests : BunitTest
    {
        /// <summary>
        /// MudIcon renders first an svg and then a span, both with style
        /// </summary>
        [Test]
        public void ShouldRenderIconWithStyle()
        {
            var colorStyle = "color: greenyellow;";
            var icon = Parameter(nameof(MudIcon.Icon), Icons.Material.Filled.Add);
            var style = Parameter(nameof(MudIcon.Style), colorStyle);
            var comp = Context.RenderComponent<MudIcon>(icon, style);
            comp.Markup.Trim().Should().StartWith("<svg")
                .And.Contain(Icons.Material.Filled.Add)
                .And.Contain($"style=\"{colorStyle}\"");

            icon = Parameter(nameof(MudIcon.Icon), "customicon");
            comp.SetParametersAndRender(icon, style);
            comp.Markup.Trim().Should().StartWith("<span")
                .And.Contain("customicon")
                .And.Contain($"style=\"{colorStyle}\"");
        }

        /// <summary>
        /// MudIcon should have a Title tag/attribute if specified
        /// </summary>
        [Test]
        public void ShouldRenderTitle()
        {
            var title = "Title and tooltip";
            //svg
            var icon = Parameter(nameof(MudIcon.Icon), Icons.Material.Filled.Add);
            var titleParam = Parameter(nameof(MudIcon.Title), title);
            var comp = Context.RenderComponent<MudIcon>(icon, titleParam);
            comp.Find("svg Title").TextContent.Should().Be(title);

            //class
            icon = Parameter(nameof(MudIcon.Icon), "customicon");
            comp.SetParametersAndRender(icon, titleParam);
            comp.Markup.Trim().Should().StartWith("<span")
                .And.Contain("customicon")
                .And.Contain($"title=\"{title}\"");
        }

        [Test]
        public void ShouldParseCorrectSyntax()
        {
            var comp = Context.RenderComponent<MudIcon>(parameters =>
                parameters.Add(parameter => parameter.Icon, "material-symbols-outlined/database"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined\" aria-hidden=\"true\" role=\"img\">database</span>");
        }

        [Test]
        public void ShouldNotParseWhenWrongSyntax()
        {
            var comp = Context.RenderComponent<MudIcon>(parameters =>
                parameters.Add(parameter => parameter.Icon, "material-symbols-outlined(database)"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined(database)\" aria-hidden=\"true\" role=\"img\"></span>");
        }

        [Test]
        public void ShouldNotParseWhenEmpty()
        {
            var comp = Context.RenderComponent<MudIcon>(parameters =>
                parameters.Add(parameter => parameter.Icon, string.Empty));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium \" aria-hidden=\"true\" role=\"img\"></span>");
        }

        [Test]
        public void ShouldUseChildContentWhenAssigned()
        {
            var comp = Context.RenderComponent<MudIcon>(parameters =>
                parameters
                    .Add(parameter => parameter.Icon, "material-symbols-outlined")
                    .AddChildContent("database"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined\" aria-hidden=\"true\" role=\"img\">database</span>");
        }

        [Test]
        public void ShouldBeEmptyChildContent()
        {
            var comp = Context.RenderComponent<MudIcon>(parameters =>
                parameters
                    .Add(parameter => parameter.Icon, "material-symbols-outlined"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined\" aria-hidden=\"true\" role=\"img\"></span>");
        }
    }
}
