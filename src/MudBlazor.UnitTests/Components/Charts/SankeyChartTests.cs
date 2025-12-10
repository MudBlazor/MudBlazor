using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class SankeyChartTests : BunitTest
    {
        [Test]
        [SuppressMessage("Performance", "SYSLIB1045:Convert to \'GeneratedRegexAttribute\'.")]
        public void ChartRendersCorrectly()
        {
            var edges = GetEdges();
            var sankey = RenderSankey(edges);
            var markup = sankey.Markup;

            // Parent
            sankey.Markup.Should().Contain("mud-chart");

            // Edges
            Regex.Matches(markup, "<linearGradient").Count.Should().Be(edges.Count);
            Regex.Matches(markup, "<path").Count.Should().Be(edges.Count);
            Regex.Matches(markup, "stop-color=\"#9E9E9E\"").Count.Should().Be(0); // Ensure the parent color palette is used
            sankey.Markup.Should().Contain("<path d=\"M19.99,18 C167.5,18 167.5,12 315.01,12 L315.01,116.6667 C167.5,116.6667 167.5,122.6667 19.99,122.6667 Z\" fill=\"url(#gradient_");
            sankey.Markup.Should().Contain(")\" opacity=\"0.5\" filter=\"\"");

            // Nodes
            Regex.Matches(markup, "<rect").Count.Should().Be(12);
            Regex.Matches(markup, "fill=\"#9E9E9E\"").Count.Should().Be(0); // Ensure the parent color palette is used
            sankey.Markup.Should().Contain("<rect x=\"315\" y=\"128.6667\" width=\"10\" height=\"104.6667\" fill=\"#FFC400\"");

            // Tooltips
            Regex.Matches(markup, "<g class=\"svg-tooltip\"").Count.Should().Be(6);
            sankey.Markup.Should().Contain("<tspan x=\"310\" dy=\"-.3em\">Chihuahua (10)</tspan>");
        }

        [Test]
        public void EmptyData()
        {
            var sankey = Context.Render<Sankey<double>>();
            sankey.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void ValidData()
        {
            var edges = GetEdges();
            var sankey = RenderSankey(edges);

            // 3 nodes and 2 edges
            sankey.FindAll("svg > rect").Count.Should().Be(6);
            sankey.FindAll("svg > path").Count.Should().Be(6);
        }

        [Test]
        public void InvalidDataNodeWidth()
        {
            var edges = GetEdges();
            var options = new SankeyChartOptions { NodeWidth = -1 };

            Assert.DoesNotThrow(() => RenderSankey(edges, options));
        }

        [Test]
        public void InvalidDataMinVerticalSpacing()
        {
            var edges = GetEdges();
            var options = new SankeyChartOptions { MinVerticalSpacing = -1 };

            Assert.DoesNotThrow(() => RenderSankey(edges, options));
        }

        private static List<SankeyEdge<double>> GetEdges()
        {
            var edges = new List<SankeyEdge<double>>
            {
                new("Dogs", "Dachshund", 10),
                new("Dogs", "Bernese", 10),
                new("Dogs", "Chihuahua", 10),
                new("Dachshund", "Good boy", 10),
                new("Bernese", "Good boy", 10),
                new("Chihuahua", "Pure evil", 10)
            };

            return edges;
        }

        private IRenderedComponent<MudChart<double>> RenderSankey(List<SankeyEdge<double>> edges, SankeyChartOptions options = null)
        {
            var result = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Sankey)
                .Add(p => p.ChartSeries, [new() { Name = "Sankey Test", Data = edges }])
                .Add(p => p.ChartOptions, options ?? new SankeyChartOptions()));

            return result;
        }
    }
}
