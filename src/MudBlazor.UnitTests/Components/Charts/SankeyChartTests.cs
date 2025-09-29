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
            var sankey = Context.RenderComponent<Sankey>();
            sankey.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void ValidData()
        {
            var (nodes, edges) = GetNodesAndEdges();
            var sankey = RenderSankey(nodes, edges);

            // 3 nodes and 2 edges
            sankey.FindAll("svg > rect").Count.Should().Be(3);
            sankey.FindAll("svg > path").Count.Should().Be(2);
        }

        [Test]
        public void InvalidDataOnlyNodes()
        {
            var (nodes, _) = GetNodesAndEdges();
            RenderSankey(nodes, []);
        }

        [Test]
        public void InvalidDataOnlyEdges()
        {
            var (_, edges) = GetNodesAndEdges();

            Assert.Throws<ArgumentException>(() => RenderSankey([], edges));
        }

        [Test]
        public void InvalidDataDuplicateName()
        {
            var (nodes, edges) = GetNodesAndEdges();
            nodes[1].Name = "Node 10";

            Assert.Throws<ArgumentException>(() => RenderSankey(nodes, edges));
        }

        [Test]
        public void InvalidDataNotExistingNode()
        {
            var (nodes, edges) = GetNodesAndEdges();
            edges[0].Source = "Node 187";

            Assert.Throws<ArgumentException>(() => RenderSankey(nodes, edges));
        }

        [Test]
        public void InvalidDataNodeWidth()
        {
            var (nodes, edges) = GetNodesAndEdges();
            var options = new NodeChartOptions { NodeWidth = -1 };

            RenderSankey(nodes, edges, options);
        }

        [Test]
        public void InvalidDataMinVerticalSpacing()
        {
            var (nodes, edges) = GetNodesAndEdges();
            var options = new NodeChartOptions { MinVerticalSpacing = -1 };

            RenderSankey(nodes, edges, options);
        }

        private static (List<SankeyChartNode> Nodes, List<SankeyChartEdge> Edges) GetNodesAndEdges()
        {
            var nodes = new List<SankeyChartNode> { new("Node 10", 0), new("Node 11", 0), new("Node 20", 1), };
            var edges = new List<SankeyChartEdge> { new("Node 10", "Node 20", 10.5), new("Node 11", "Node 20", 5), };

            return (nodes, edges);
        }

        private IRenderedComponent<Sankey> RenderSankey(List<SankeyChartNode> nodes, List<SankeyChartEdge> edges, NodeChartOptions options = null)
        {
            var result = Context.RenderComponent<Sankey>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Sankey)
                .Add(p => p.Nodes, nodes)
                .Add(p => p.Edges, edges)
                .Add(p => p.NodeChartOptions, options ?? new NodeChartOptions()));

            return result;
        }
    }
}
