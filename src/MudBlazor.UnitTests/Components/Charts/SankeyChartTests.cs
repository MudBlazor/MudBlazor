using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class SankeyChartTests : BunitTest
    {
        [SetUp] public void Init() { }

        [Test]
        public void EmptyData()
        {
            var sankey = Context.RenderComponent<Sankey<double>>();
            sankey.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void ValidData()
        {
            var edges = GetEdges();
            var sankey = RenderSankey(edges);

            // 3 nodes and 2 edges
            sankey.FindAll("svg > rect").Count.Should().Be(3);
            sankey.FindAll("svg > path").Count.Should().Be(2);
        }

        [Test]
        public void InvalidDataNodeWidth()
        {
            var edges = GetEdges();
            var options = new SankeyChartOptions { NodeWidth = -1 };

            RenderSankey(edges, options);
        }

        [Test]
        public void InvalidDataMinVerticalSpacing()
        {
            var edges = GetEdges();
            var options = new SankeyChartOptions { MinVerticalSpacing = -1 };

            RenderSankey(edges, options);
        }

        private static List<SankeyEdge<double>> GetEdges()
        {
            var edges = new List<SankeyEdge<double>> { new("Node 10", "Node 20", 10.5), new("Node 11", "Node 20", 5), };

            return edges;
        }

        private IRenderedComponent<Sankey<double>> RenderSankey(List<SankeyEdge<double>> edges, SankeyChartOptions options = null)
        {
            var result = Context.RenderComponent<Sankey<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Sankey)
                .Add(p => p.ChartSeries, [new() { Name = "Sankey Test", Data = edges }])
                .Add(p => p.ChartOptions, options ?? new SankeyChartOptions()));

            return result;
        }
    }
}
