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
            var (nodes, edges) = GetNodesAndEdges();
            var sankey = RenderSankey(nodes, edges);
            var markup = sankey.Markup;

            // Parent
            sankey.Markup.Should().Contain("mud-chart");

            // Edges
            Regex.Matches(markup, "<linearGradient").Count.Should().Be(edges.Count);
            Regex.Matches(markup, "<path").Count.Should().Be(edges.Count);
            Regex.Matches(markup, "stop-color=\"#9E9E9E\"").Count.Should().Be(0); // Ensure the parent color palette is used
            sankey.Markup.Should().Contain("<path d=\"M19.99,18 C167.5,18 167.5,12 315.01,12 L315.01,116.6667 C167.5,116.6667 167.5,122.6667 19.99,122.6667 Z\" fill=\"url(#gradient_");
            sankey.Markup.Should().Contain(")\" opacity=\"0.5\" filter=\"\" blazor:onmouseover=\"5\" blazor:onmouseout=\"6\">");

            // Nodes
            Regex.Matches(markup, "<rect").Count.Should().Be(nodes.Count * 2);
            Regex.Matches(markup, "fill=\"#9E9E9E\"").Count.Should().Be(0); // Ensure the parent color palette is used
            sankey.Markup.Should().Contain("<rect x=\"315\" y=\"128.6667\" width=\"10\" height=\"104.6667\" fill=\"#FFC400\" filter=\"\" blazor:onmouseover=\"19\" " +
                                           "blazor:onmouseout=\"20\" blazor:onclick=\"21\">");

            // Tooltips
            Regex.Matches(markup, "<g class=\"svg-tooltip\"").Count.Should().Be(nodes.Count);
            sankey.Markup.Should().Contain("<tspan x=\"310\" dy=\"-.3em\">Chihuahua (10)</tspan>");
            foreach (var node in nodes)
            {
                sankey.Markup.Should().Contain($">{node.Name} (");
            }
        }

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

            sankey.FindAll("svg > rect").Count.Should().Be(nodes.Count);
            sankey.FindAll("svg > path").Count.Should().Be(edges.Count);
        }

        [Test]
        public void InvalidDataOnlyNodes()
        {
            var (nodes, _) = GetNodesAndEdges();
            Assert.DoesNotThrow(() => RenderSankey(nodes, []));
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
            nodes[1].Name = nodes[0].Name;

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

            Assert.DoesNotThrow(() => RenderSankey(nodes, edges, options));
        }

        [Test]
        public void InvalidDataMinVerticalSpacing()
        {
            var (nodes, edges) = GetNodesAndEdges();
            var options = new NodeChartOptions { MinVerticalSpacing = -1 };

            Assert.DoesNotThrow(() => RenderSankey(nodes, edges, options));
        }

        private static (List<SankeyChartNode> Nodes, List<SankeyChartEdge> Edges) GetNodesAndEdges()
        {
            var nodes = new List<SankeyChartNode>
            {
                new("Dogs", 0),
                new("Dachshund", 1),
                new("Bernese", 1),
                new("Chihuahua", 1),
                new("Good boy", 2),
                new("Pure evil", 2)
            };

            var edges = new List<SankeyChartEdge>
            {
                new("Dogs", "Dachshund", 10),
                new("Dogs", "Bernese", 10),
                new("Dogs", "Chihuahua", 10),
                new("Dachshund", "Good boy", 10),
                new("Bernese", "Good boy", 10),
                new("Chihuahua", "Pure evil", 10)
            };

            return (nodes, edges);
        }

        private IRenderedComponent<MudChart> RenderSankey(List<SankeyChartNode> nodes, List<SankeyChartEdge> edges, NodeChartOptions options = null)
        {
            var result = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Sankey)
                .Add(p => p.Nodes, nodes)
                .Add(p => p.Edges, edges)
                .Add(p => p.NodeChartOptions, options ?? new NodeChartOptions()));

            return result;
        }
    }
}
