// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    [SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.")]
    public class SankeyChartTests : BunitTest
    {
        [Test]
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

        [Test]
        [CancelAfter(5000)]
        public void CircularEdgesDoNotHang()
        {
            // A cycle (A -> B -> A) has no root node, so every node falls back to a "source".
            // CalculateNodeColumns previously enqueued targets forever and hung the render;
            // it must now terminate and still draw both nodes.
            var edges = new List<SankeyEdge<double>>
            {
                new("A", "B", 10),
                new("B", "A", 5)
            };

            var sankey = RenderSankey(edges);

            sankey.FindAll("svg > rect").Count.Should().Be(2);
        }

        [Test]
        public void ValidNodeWidth_IsReflectedInRectWidth()
        {
            var edges = MultiColumnEdges();
            var options = new SankeyChartOptions { NodeWidth = 25, MinVerticalSpacing = 30 };

            var comp = RenderSankey(edges, options);

            comp.Markup.Should().Contain("width=\"25\"");
            comp.FindAll("svg > rect").Count.Should().Be(5);
        }

        [Test]
        public void MultiColumnLayout_SpreadsNodesAcrossColumns()
        {
            var edges = MultiColumnEdges();
            var comp = RenderSankey(edges);

            var sankey = comp.FindComponent<Sankey<double>>().Instance;
            // A=0, B=1, C/D=2, E=3 : longest path is 3, so 4 distinct columns.
            sankey.Nodes.Select(n => n.Column).Distinct().Count().Should().Be(4);
            sankey.Nodes.Select(n => n.Name).Should().BeEquivalentTo(["A", "B", "C", "D", "E"]);
        }

        [Test]
        public void NodeOverrides_ApplyColorAndAddMissingNodes()
        {
            var edges = FanEdges();
            var overrideColor = new MudColor("#123456");
            var options = new SankeyChartOptions
            {
                NodeOverrides =
                [
                    // Override an existing node's color.
                    new SankeyNode("Dogs", 0, overrideColor),
                    // Add a node that has no edge.
                    new SankeyNode("Lonely", 2, new MudColor("#abcdef")),
                ],
            };

            var comp = RenderSankey(edges, options);
            var sankey = comp.FindComponent<Sankey<double>>().Instance;

            sankey.Nodes.Select(n => n.Name).Should().Contain("Lonely");
            sankey.Nodes.First(n => n.Name == "Dogs").Color.ToString(MudColorOutputFormats.HexA)
                .Should().Be(overrideColor.ToString(MudColorOutputFormats.HexA));

            // The node rect for Dogs is rendered with the overridden color.
            comp.Markup.Should().Contain(overrideColor.ToString(MudColorOutputFormats.HexA));
        }

        [Test]
        public async Task NodeClick_SetsSelectedIndex()
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges);

            comp.FindAll("svg > rect").Count.Should().BeGreaterThan(0);

            await comp.Find("svg > rect").ClickAsync(new());

            comp.Instance.SelectedIndex.Should().BeGreaterThanOrEqualTo(0);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task NodeHover_AppliesGlowOnlyWhenHighlightEnabled(bool highlightOnHover)
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges, new SankeyChartOptions { HighlightOnHover = highlightOnHover });

            await comp.Find("svg > rect").MouseOverAsync(new());

            var glowsOnHover = Regex.Matches(comp.Markup, "filter=\"url\\(#glow_").Count;
            if (highlightOnHover)
            {
                glowsOnHover.Should().BeGreaterThan(0);
            }
            else
            {
                glowsOnHover.Should().Be(0);
            }

            await comp.Find("svg > rect").MouseOutAsync(new());
            comp.Render();
            // After mouse out the active glow filter is gone from node rects.
            Regex.Matches(comp.Markup, "filter=\"url\\(#glow_").Count.Should().Be(0);
        }

        [Test]
        public async Task EdgeHover_HighlightsActiveEdge_AndShowsEdgeTooltip()
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges, new SankeyChartOptions { HighlightOnHover = true });

            await comp.Find("svg > path").MouseOverAsync(new());

            // Hovering an edge highlights it and renders its tooltip.
            comp.Markup.Should().Contain("filter=\"url(#glow_");
            comp.FindComponents<ChartTooltip>().Count.Should().BeGreaterThan(0);

            await comp.Find("svg > path").MouseOutAsync(new());
            comp.Render();
            // After mouse out the glow highlight is cleared from edges.
            Regex.Matches(comp.Markup, "filter=\"url\\(#glow_").Count.Should().Be(0);
        }

        [Test]
        public void ShowEdgeLabels_RendersOneEdgeTooltipPerEdge()
        {
            var edges = FanEdges();
            var options = new SankeyChartOptions { ShowEdgeLabels = true, ShowLabels = false };
            var comp = RenderSankey(edges, options);

            Regex.Matches(comp.Markup, "<g class=\"svg-tooltip\"").Count.Should().Be(edges.Count);
        }

        [Test]
        public void CustomEdgeLabelSymbol_IsUsedInEdgeNames()
        {
            var edges = FanEdges();
            var options = new SankeyChartOptions { ShowEdgeLabels = true, EdgeLabelSymbol = "->" };
            var comp = RenderSankey(edges, options);

            comp.Markup.Should().Contain("Dogs -&gt; Dachshund");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ShowNodeValues_GatesParenthesisedTotalInLabels(bool showNodeValues)
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges, new SankeyChartOptions { ShowLabels = true, ShowNodeValues = showNodeValues });

            if (showNodeValues)
            {
                comp.Markup.Should().Contain("Dachshund (10)");
            }
            else
            {
                comp.Markup.Should().NotContain("Dachshund (10)");
                comp.Markup.Should().Contain("Dachshund");
            }
        }

        [Test]
        public void ShowLabels_RendersNodeTooltipForEveryNode()
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges, new SankeyChartOptions { ShowLabels = true, ShowEdgeLabels = false });

            // One node tooltip per node (Dogs + 3 breeds = 4).
            Regex.Matches(comp.Markup, "<g class=\"svg-tooltip\"").Count.Should().Be(4);
            comp.Markup.Should().Contain(">Dogs (60)<");
        }

        [Test]
        public void TooltipTemplate_RendersCustomEdgeMarkup()
        {
            var edges = FanEdges();
            RenderFragment<(SvgPath Segment, string Color)> template = ctx => builder =>
            {
                builder.OpenElement(0, "text");
                builder.AddAttribute(1, "class", "custom-edge-template");
                builder.AddContent(2, ((EdgePath)ctx.Segment).Name);
                builder.CloseElement();
            };

            var comp = RenderSankey(edges, new SankeyChartOptions { ShowEdgeLabels = true },
                p => p.Add(c => c.TooltipTemplate, template));

            comp.Markup.Should().Contain("custom-edge-template");
            // The custom template path wraps the content in a mud-chart-tooltip group.
            comp.Markup.Should().Contain("class=\"mud-chart-tooltip\"");
        }

        [Test]
        public void HideNodesWithNoEdges_RemovesIsolatedNodes()
        {
            var edges = FanEdges();
            var options = new SankeyChartOptions
            {
                HideNodesWithNoEdges = true,
                NodeOverrides = [new SankeyNode("Isolated", 1)],
            };

            var comp = RenderSankey(edges, options);

            // The isolated node exists in the node set but is not drawn as a rect.
            var sankey = comp.FindComponent<Sankey<double>>().Instance;
            sankey.Nodes.Select(n => n.Name).Should().Contain("Isolated");

            // 4 connected nodes (Dogs + 3 breeds) are rendered, isolated one is not.
            comp.FindAll("svg > rect").Count.Should().Be(4);
        }

        [Test]
        public void HideNodesSmallerThan_FiltersByValue()
        {
            var edges = FanEdges(); // weights 10, 20, 30
            var options = new SankeyChartOptions { HideNodesSmallerThan = 15 };

            var comp = RenderSankey(edges, options);

            // Dachshund (value 10) is filtered out, leaving Dogs + Bernese + Chihuahua = 3 rects.
            comp.FindAll("svg > rect").Count.Should().Be(3);
            comp.Markup.Should().NotContain(">Dachshund");
        }

        [Test]
        [TestCase(AggregationOption.GroupByDataSet)]
        [TestCase(AggregationOption.GroupByLabel)]
        public void Aggregation_BuildsEdgesBetweenSeriesAndLabels(AggregationOption aggregation)
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Alpha", Data = new double[] { 5, 3 } },
                new() { Name = "Beta", Data = new double[] { 2, 8 } },
            };
            var labels = new[] { "L1", "L2" };

            var comp = Context.Render<MudChart<double>>(p => p
                .Add(c => c.ChartType, ChartType.Sankey)
                .Add(c => c.ChartSeries, series)
                .Add(c => c.ChartLabels, labels)
                .Add(c => c.ChartOptions, new SankeyChartOptions { AggregationOption = aggregation }));

            var sankey = comp.FindComponent<Sankey<double>>().Instance;
            sankey.Nodes.Select(n => n.Name).Should().Contain(["Alpha", "Beta", "L1", "L2"]);
            comp.FindAll("svg > path").Count.Should().Be(4);

            // The two aggregations differ in edge direction: GroupByDataSet flows series -> label,
            // GroupByLabel flows label -> series. Source nodes always sit in an earlier column.
            var edge = sankey.Edges.First();
            var sourceColumn = sankey.Nodes.First(n => n.Name == edge.Source).Column;
            var targetColumn = sankey.Nodes.First(n => n.Name == edge.Target).Column;
            sourceColumn.Should().BeLessThan(targetColumn);
            if (aggregation == AggregationOption.GroupByDataSet)
            {
                edge.Source.Should().BeOneOf("Alpha", "Beta");
                edge.Target.Should().BeOneOf("L1", "L2");
            }
            else
            {
                edge.Source.Should().BeOneOf("L1", "L2");
                edge.Target.Should().BeOneOf("Alpha", "Beta");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ChartPalette_AppliesColorsOrFallsBackToGray(bool customPalette)
        {
            var edges = FanEdges();
            var options = customPalette
                ? new SankeyChartOptions { ChartPalette = ["#112233", "#445566"] }
                : new SankeyChartOptions { ChartPalette = [] };

            var comp = RenderSankey(edges, options);

            if (customPalette)
            {
                // Palette colors cycle across node rects.
                comp.Markup.Should().Contain("fill=\"#112233\"");
                comp.Markup.Should().Contain("fill=\"#445566\"");
            }
            else
            {
                // With no palette, node rects fall back to the default gray color.
                comp.Markup.Should().Contain($"fill=\"{Colors.Gray.Default}\"");
            }
        }

        [Test]
        public void MatchBoundsToSize_UsesPixelWidthAndHeightForViewBox()
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges, new SankeyChartOptions(), p => p
                .Add(c => c.MatchBoundsToSize, true)
                .Add(c => c.Width, "500px")
                .Add(c => c.Height, "300px"));

            comp.Markup.Should().Contain("viewBox=\"0 0 500 300\"");
        }

        [Test]
        public void EdgeOpacity_IsAppliedToPaths()
        {
            var edges = FanEdges();
            var comp = RenderSankey(edges, new SankeyChartOptions { EdgeOpacity = 0.8 });

            comp.Markup.Should().Contain("opacity=\"0.8\"");
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

        // A->B, B->C, B->D, C->E : forces a 4-column layout (A=0, B=1, C/D=2, E=3)
        private static List<SankeyEdge<double>> MultiColumnEdges() =>
        [
            new("A", "B", 10),
            new("B", "C", 6),
            new("B", "D", 4),
            new("C", "E", 6),
        ];

        // Simple two-column fan-out used by several tests.
        private static List<SankeyEdge<double>> FanEdges() =>
        [
            new("Dogs", "Dachshund", 10),
            new("Dogs", "Bernese", 20),
            new("Dogs", "Chihuahua", 30),
        ];

        private IRenderedComponent<MudChart<double>> RenderSankey(
            IReadOnlyList<SankeyEdge<double>> edges,
            SankeyChartOptions options = null,
            Action<ComponentParameterCollectionBuilder<MudChart<double>>> extra = null)
        {
            return Context.Render<MudChart<double>>(parameters =>
            {
                parameters
                    .Add(p => p.ChartType, ChartType.Sankey)
                    .Add(p => p.ChartSeries, [new() { Name = "Sankey Test", Data = edges.ToList() }])
                    .Add(p => p.ChartOptions, options ?? new SankeyChartOptions());
                extra?.Invoke(parameters);
            });
        }
    }
}
