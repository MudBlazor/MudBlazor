using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class IconTests : BunitTest
    {
        /// <summary>
        /// MudIcon renders first an svg and then a span, both with style
        /// </summary>
        [Test]
        public async Task ShouldRenderIconWithStyle()
        {
            var colorStyle = "color: greenyellow;";
            var comp = Context.Render<MudIcon>(parameters => parameters
                .Add(x => x.Icon, Icons.Material.Filled.Add)
                .Add(x => x.Style, colorStyle));
            comp.Markup.Trim().Should().StartWith("<svg")
                .And.Contain(Icons.Material.Filled.Add)
                .And.Contain($"style=\"{colorStyle}\"");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Icon, "customicon")
                .Add(x => x.Style, colorStyle));
            comp.Markup.Trim().Should().StartWith("<span")
                .And.Contain("customicon")
                .And.Contain($"style=\"{colorStyle}\"");
        }

        /// <summary>
        /// MudIcon should have a Title tag/attribute if specified
        /// </summary>
        [Test]
        public async Task ShouldRenderTitle()
        {
            var title = "Title and tooltip";
            //svg
            var comp = Context.Render<MudIcon>(parameters => parameters
                .Add(x => x.Icon, Icons.Material.Filled.Add)
                .Add(x => x.Title, title));
            comp.Find("svg Title").TextContent.Should().Be(title);

            //class
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Icon, "customicon")
                .Add(x => x.Title, title));
            comp.Markup.Trim().Should().StartWith("<span")
                .And.Contain("customicon")
                .And.Contain($"title=\"{title}\"");
        }

        [Test]
        public void ShouldParseCorrectSyntax()
        {
            var comp = Context.Render<MudIcon>(parameters =>
                parameters.Add(parameter => parameter.Icon, "material-symbols-outlined/database"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined\" aria-hidden=\"true\" role=\"img\">database</span>");
        }

        [Test]
        public void ShouldNotParseWhenWrongSyntax()
        {
            var comp = Context.Render<MudIcon>(parameters =>
                parameters.Add(parameter => parameter.Icon, "material-symbols-outlined(database)"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined(database)\" aria-hidden=\"true\" role=\"img\"></span>");
        }

        [Test]
        public void ShouldNotParseWhenEmpty()
        {
            var comp = Context.Render<MudIcon>(parameters =>
                parameters.Add(parameter => parameter.Icon, string.Empty));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium \" aria-hidden=\"true\" role=\"img\"></span>");
        }

        [Test]
        public void ShouldUseChildContentWhenAssigned()
        {
            var comp = Context.Render<MudIcon>(parameters =>
                parameters
                    .Add(parameter => parameter.Icon, "material-symbols-outlined")
                    .AddChildContent("database"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined\" aria-hidden=\"true\" role=\"img\">database</span>");
        }

        [Test]
        public void ShouldBeEmptyChildContent()
        {
            var comp = Context.Render<MudIcon>(parameters =>
                parameters
                    .Add(parameter => parameter.Icon, "material-symbols-outlined"));

            comp.Markup.Should().Be("<span class=\"mud-icon-root mud-icon-size-medium material-symbols-outlined\" aria-hidden=\"true\" role=\"img\"></span>");
        }
    }
}
