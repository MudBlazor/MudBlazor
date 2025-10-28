using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays data as nodes connected by weighted edges.
    /// </summary>
    partial class Sankey<T> : MudChartBase<T, SankeyChartOptions>, IDisposable where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        private DotNetObjectReference<Sankey<T>>? _dotNetObjectReference;
        protected ElementReference _elementReference;

        private const double Epsilon = 1e-6;
        private const double BoundWidthDefault = 650;
        private const double BoundHeightDefault = 350;
        private const double HorizontalPadding = 10;

        private ElementSize? _elementSize;

        private double _boundWidth = BoundWidthDefault;
        private double _boundHeight = BoundHeightDefault;

        /// <summary>
        /// The collection of nodes that represent the points within the Sankey diagram. 
        /// Each node typically corresponds to a source or target 
        /// </summary>
        /// <remarks>
        /// Nodes define the visible anchors of the Sankey flow. Each node should have a unique 
        /// identifier to ensure correct edge linkage and layout calculation.
        /// </remarks>
        public HashSet<SankeyNode> Nodes { get; set; } = [];

        /// <summary>
        /// The collection of edges that represent the flows between nodes in the Sankey diagram. 
        /// Each edge defines a source node, a target node, and an associated value (or weight) that determines the flow thickness.
        /// </summary>
        public HashSet<SankeyEdge<T>> Edges { get; set; } = [];

        private Dictionary<string, NodeRect> NodeRects { get; } = [];
        private Dictionary<string, double> NodeValues { get; set; } = [];
        private List<EdgePath> EdgePaths { get; } = [];
        private string? ActiveNode { get; set; }
        private string? ActiveEdge { get; set; }

        private Dictionary<string, SankeyNode> _nodeLookup = [];

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart<T>? MudChartParent { get; set; }

        [DynamicDependency(nameof(OnElementSizeChanged))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElementSize))]
        public Sankey()
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            var seriesData = AggregateSeriesData(ChartOptions?.AggregationOption ?? AggregationOption.None);

            Edges = Sankey<T>.EnsureUniqueEdges(seriesData);
            Nodes = GenerateNodesFromEdges();

            if (ChartOptions?.NodeOverrides is { Count: > 0 } overrides)
            {
                _nodeLookup = overrides.ToDictionary(d => d.Name);

                Nodes = [.. Nodes.Select(n =>
                {
                    if (_nodeLookup.TryGetValue(n.Name, out var definition))
                    {
                        return n with { Column = definition.Column, Color = definition.Color ?? n.Color };
                    }
                    return n;
                })];
            }

            // Assert input data
            var nodeGroups = Nodes.GroupBy(e => e.Name).ToList();
            if (nodeGroups.Any(grp => grp.Count() > 1))
            {
                throw new ArgumentException("All nodes must have unique names");
            }

            var edgeWithInvalidNode = Edges.FirstOrDefault(e => Nodes.All(n => n.Name != e.Source) || Nodes.All(n => n.Name != e.Target));
            if (edgeWithInvalidNode != null)
            {
                throw new ArgumentException($"Edge {edgeWithInvalidNode.Source} => {edgeWithInvalidNode.Target} specifies an non-existing node");
            }

            if (Nodes.Count != 0)
                RebuildChart();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, _elementReference);
                OnElementSizeChanged(elementSize);
            }
        }

        public override void RebuildChart()
        {
            SetBounds();
            NodeValues = GetAllNodeValues();
            var (maxColumnValue, relativeBoundHeight) = GenerateNodeRects();
            GenerateEdgePaths(maxColumnValue, relativeBoundHeight);
        }

        private void SetBounds()
        {
            _boundWidth = BoundWidthDefault;
            _boundHeight = BoundHeightDefault;

            if (MatchBoundsToSize)
            {
                if (_elementSize is not null)
                {
                    _boundWidth = _elementSize.Width;
                    _boundHeight = _elementSize.Height;
                }
                else if (Width.EndsWith("px")
                    && Height.EndsWith("px")
                    && double.TryParse(Width.AsSpan(0, Width.Length - 2), out var width)
                    && double.TryParse(Height.AsSpan(0, Height.Length - 2), out var height))
                {
                    _boundWidth = width;
                    _boundHeight = height;
                }
            }
        }

        /// <summary>
        /// Generates nodes from edges by building a directed graph and calculating columns
        /// based on the longest path from source nodes.
        /// </summary>
        private HashSet<SankeyNode> GenerateNodesFromEdges()
        {
            if (Edges.Count == 0)
                return [];

            // Get all unique node names
            var allNodeNames = Edges.SelectMany(e => new[] { e.Source, e.Target }).ToHashSet();

            // Build adjacency list for the graph
            var outgoingEdges = Edges
                .GroupBy(e => e.Source)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Target).ToList());

            // Find all source nodes (nodes with no incoming edges)
            var targetNodes = Edges.Select(e => e.Target).ToHashSet();
            var sourceNodes = allNodeNames.Except(targetNodes).ToList();

            // If no source nodes found (circular graph), use all nodes
            if (sourceNodes.Count == 0)
                sourceNodes = [.. allNodeNames];

            // Calculate column for each node using BFS
            var nodeColumns = Sankey<T>.CalculateNodeColumns(allNodeNames, outgoingEdges, sourceNodes);

            // Create nodes with calculated columns
            return [.. allNodeNames.Select(name => new SankeyNode(name, nodeColumns[name]))];
        }

        /// <summary>
        /// Calculates the column index for each node based on the longest path from source nodes.
        /// Uses a breadth-first traversal approach.
        /// </summary>
        private static Dictionary<string, int> CalculateNodeColumns(HashSet<string> allNodes, Dictionary<string, List<string>> outgoingEdges, List<string> sourceNodes)
        {
            var nodeColumns = new Dictionary<string, int>();

            // Initialize all nodes at column 0
            foreach (var node in allNodes)
            {
                nodeColumns[node] = 0;
            }

            // BFS to calculate maximum depth for each node
            var queue = new Queue<(string Node, int Column)>();

            // Start with all source nodes at column 0
            foreach (var source in sourceNodes)
            {
                queue.Enqueue((source, 0));
            }

            while (queue.Count > 0)
            {
                var (currentNode, currentColumn) = queue.Dequeue();

                // Update column to maximum depth seen so far
                nodeColumns[currentNode] = Math.Max(nodeColumns[currentNode], currentColumn);

                if (!outgoingEdges.TryGetValue(currentNode, out var targets))
                    continue;

                foreach (var target in targets)
                {
                    var targetColumn = currentColumn + 1;

                    // Always enqueue to ensure we find the longest path
                    queue.Enqueue((target, targetColumn));
                }
            }

            return nodeColumns;
        }

        private List<ChartSeries<T>> AggregateSeriesData(AggregationOption aggregation)
        {
            if (ChartSeries is null || ChartSeries.Count == 0 || !ChartSeries.Any(x => x.Visible))
                return [];

            if (aggregation == AggregationOption.None)
                return ChartSeries;

            var maxCategoryLength = aggregation == AggregationOption.GroupByLabel
                    ? GetMaxCategoryLengthForLabelGrouping()
                    : ChartSeries.Count;

            var aggregated = new ChartSeries<T>[maxCategoryLength];

            return aggregation switch
            {
                AggregationOption.GroupByLabel => AggregateByLabel(aggregated),
                AggregationOption.GroupByDataSet => AggregateByDataSet(aggregated),
                _ => throw new ArgumentOutOfRangeException(nameof(aggregation), $"Unsupported aggregation: {aggregation}")
            };
        }

        private int GetMaxCategoryLengthForLabelGrouping()
        {
            if (ChartLabels.Length > 0)
                return ChartLabels.Length;

            return ChartSeries.Where(x => x.Data?.Values != null).DefaultIfEmpty()
                              .Max(x => x?.Data?.Values.Count ?? 0);
        }

        private List<ChartSeries<T>> AggregateByLabel(ChartSeries<T>[] aggregated)
        {
            var result = new List<ChartSeries<T>>();

            foreach (var series in ChartSeries.Where(s => s.Visible))
            {
                var data = new List<(SankeyLink, T)>();
                var values = series.Data?.Values ?? [];

                for (var i = 0; i < values.Count; i++)
                {
                    if (i < aggregated.Length)
                    {
                        var link = new SankeyLink(ChartLabels[i], series.Name);
                        data.Add((link, values[i]));
                    }
                }

                result.Add(new ChartSeries<T>
                {
                    Name = series.Name,
                    Data = data.ToArray(),
                    Visible = series.Visible
                });
            }

            return result;
        }

        private List<ChartSeries<T>> AggregateByDataSet(ChartSeries<T>[] aggregated)
        {
            var result = new List<ChartSeries<T>>();
            var chartSeries = ChartSeries.Take(aggregated.Length);

            foreach (var (series, index) in chartSeries.Select((s, i) => (s, i)))
            {
                var data = new List<(SankeyLink, T)>();
                var values = series.Data?.Values ?? [];

                if (!series.Visible) continue;

                for (var i = 0; i < values.Count; i++)
                {
                    var link = new SankeyLink(series.Name, ChartLabels[i]);
                    data.Add((link, values[i]));
                }

                result.Add(new ChartSeries<T>
                {
                    Name = series.Name,
                    Data = data.ToArray(),
                    Visible = series.Visible
                });
            }

            return result;
        }

        private static HashSet<SankeyEdge<T>> EnsureUniqueEdges(List<ChartSeries<T>> chartSeries)
        {
            var unique = new HashSet<SankeyEdge<T>>();

            foreach (var series in chartSeries.Where(s => s.Visible))
            {
                var edges = series.Data.Points.Select(x =>
                {
                    if (x.X is SankeyLink link)
                        return new SankeyEdge<T>(link.Source, link.Target, x.Y);

                    throw new ArgumentException("Invalid Sankey data point provided");
                }).ToHashSet();

                unique.UnionWith(edges);
            }

            return unique;
        }

        private Dictionary<string, double> GetAllNodeValues()
        {
            var incoming = Edges
                .GroupBy(e => e.Target)
                .ToDictionary(grp => grp.Key, grp => grp.Aggregate(T.Zero, (sum, e) => sum + e.Weight));
            var outgoing = Edges
                .GroupBy(e => e.Source)
                .ToDictionary(grp => grp.Key, grp => grp.Aggregate(T.Zero, (sum, e) => sum + e.Weight));

            var nodeValues = new Dictionary<string, double>();

            foreach (var node in Nodes)
            {
                incoming.TryGetValue(node.Name, out var inValue);
                outgoing.TryGetValue(node.Name, out var outValue);
                nodeValues[node.Name] = Math.Max(double.CreateSaturating(inValue), double.CreateSaturating(outValue));
            }

            return nodeValues;
        }

        private (double MaxNodeValue, double RealtiveBoundHeight) GenerateNodeRects()
        {
            NodeRects.Clear();

            var nodesPerColumn = NormaliseNodeColumnIndices()
                .GroupBy(x => x.Column)
                .OrderBy(grp => grp.Key)
                .ToArray();
            var maxColumnValue = Nodes
                .GroupBy(n => n.Column)
                .Select(grp => grp.Sum(n => NodeValues.GetValueOrDefault(n.Name))) //.Aggregate(T.Zero, (sum, n) => double.CreateSaturating(sum) + NodeValues.GetValueOrDefault(n.Name)))
                .Max();
            var relativeNodesValuesMapping = GetNormalisedNodeValuesMapping(maxColumnValue);

            // Calculate grid sizes
            var maxRows = nodesPerColumn.Max(n => n.Count());
            var maxColumns = nodesPerColumn.Length - 1;
            var boundHeightRelativeToNodeHeight = _boundHeight - ChartOptions!.MinVerticalSpacing * maxRows;
            var boundWidthRelativeToNodeWidth = _boundWidth - ChartOptions!.NodeWidth * maxColumns - 2 * HorizontalPadding;

            // Draw all nodes column per column
            foreach (var column in nodesPerColumn)
            {
                var x = column.First().Column / (double)maxColumns * boundWidthRelativeToNodeWidth + HorizontalPadding;
                var totalRelativeColumnValue = column.Sum(n => relativeNodesValuesMapping[n]); //column.Aggregate(T.Zero, (sum, n) => sum + relativeNodesValuesMapping[n]);
                var totalVerticalSpace = _boundHeight - double.CreateSaturating(totalRelativeColumnValue) * boundHeightRelativeToNodeHeight;
                var verticalSpacing = Math.Max(totalVerticalSpace / (column.Count() + 1), ChartOptions!.MinVerticalSpacing);

                double currentY = 0;
                foreach (var node in column)
                {
                    var y = currentY + verticalSpacing;
                    var height = double.CreateSaturating(relativeNodesValuesMapping[node]) * boundHeightRelativeToNodeHeight;

                    NodeRects[node.Name] = new NodeRect(
                        Hash: node.GetHashCode(),
                        Name: node.Name,
                        X: x,
                        Y: y,
                        Width: ChartOptions!.NodeWidth,
                        Height: height,
                        Color: GetNextHexColorForNodeRect(node)
                    );

                    currentY = y + height;
                }
            }

            return (double.CreateSaturating(maxColumnValue), boundHeightRelativeToNodeHeight);
        }

        private SankeyNode[] NormaliseNodeColumnIndices()
        {
            var nodes = Nodes.ToArray();

            // Normalise column indices
            var columnMap = nodes
                .Select(n => n.Column)
                .Distinct()
                .OrderBy(c => c)
                .Select((c, index) => new { Old = c, New = index })
                .ToDictionary(x => x.Old, x => x.New);
            Array.ForEach(nodes, node => node = node with { Column = columnMap[node.Column] });

            return nodes;
        }

        private Dictionary<SankeyNode, double> GetNormalisedNodeValuesMapping(double maxColumnValue)
        {
            var result = new Dictionary<SankeyNode, double>();
            foreach (var node in Nodes)
            {
                result[node] = NodeValues.GetValueOrDefault(node.Name) / maxColumnValue;
            }

            return result;
        }

        private string GetNextHexColorForNodeRect(SankeyNode node)
        {
            if (_nodeLookup.TryGetValue(node.Name, out var definition) && definition.Color is not null)
            {
                return definition.Color.ToString(MudColorOutputFormats.HexA);
            }

            if (ChartOptions!.ChartPalette is { Length: > 0 } palette)
            {
                return palette[NodeRects.Count % palette.Length];
            }

            return Colors.Gray.Default;
        }

        private void GenerateEdgePaths(double maxColumnValue, double relativeBoundHeight)
        {
            EdgePaths.Clear();

            var index = 0;
            var edgesPerSources = Edges.GroupBy(e => e.Source).ToList();
            foreach (var sourceGrp in edgesPerSources)
            {
                if (!NodeRects.TryGetValue(sourceGrp.Key, out var rectSource)) continue;

                double startYOffset = 0;
                foreach (var edge in sourceGrp)
                {
                    if (!NodeRects.TryGetValue(edge.Target, out var rectTarget)) continue;

                    var startX = rectSource.X + rectSource.Width;
                    var startY = rectSource.Y + startYOffset;
                    var endX = rectTarget.X;
                    var endY = rectTarget.LowestIncomingNodeY;
                    var height = double.CreateSaturating(edge.Weight) / maxColumnValue * relativeBoundHeight;

                    EdgePaths.Add(new EdgePath()
                    {
                        Index = index++,
                        Name = $"{rectSource.Name} -> {rectTarget.Name} ({edge.Weight})",
                        Source = rectSource,
                        Target = rectTarget,
                        Data = BuildSankyEdgePath(
                            sourceX: startX - 0.1, // -0.1 to prevent a visible edge when setting the edge opacity to 1
                            sourceY: startY,
                            sourceHeight: height,
                            targetX: endX + 0.1, // +0.1 to prevent a visible edge when setting the edge opacity to 1
                            targetY: endY,
                            targetHeight: height
                        ),
                        LabelXValue = rectSource.Name,
                        LabelYValue = rectTarget.Name,
                        LabelX = startX + Math.Abs(startX - endX) / 2,
                        LabelY = startY + Math.Abs(startY - (endY + height)) / 2
                    });

                    startYOffset += height;
                    rectTarget.LowestIncomingNodeY += height;
                }
            }
        }

        private static string BuildSankyEdgePath(double sourceX, double sourceY, double sourceHeight, double targetX, double targetY, double targetHeight)
        {
            // Midpoints of source and target edges
            var sy0 = sourceY;
            var sy1 = sourceY + sourceHeight;
            var ty0 = targetY;
            var ty1 = targetY + targetHeight;

            // Control points for cubic Bezier curve
            const double Curvature = 0.5;
            var cx0 = sourceX + (targetX - sourceX) * Curvature;
            var cx1 = targetX - (targetX - sourceX) * Curvature;

            return $"M{ToS(sourceX)},{ToS(sy0)} " + // Top-left of source
                   $"C{ToS(cx0)},{ToS(sy0)} " + // Control point 1
                   $"{ToS(cx1)},{ToS(ty0)} " + // Control point 2
                   $"{ToS(targetX)},{ToS(ty0)} " + // Top of target
                   $"L{ToS(targetX)},{ToS(ty1)} " + // Bottom of target
                   $"C{ToS(cx1)},{ToS(ty1)} " + // Control point 2 mirrored
                   $"{ToS(cx0)},{ToS(sy1)} " + // Control point 1 mirrored
                   $"{ToS(sourceX)},{ToS(sy1)} Z"; // Bottom of source
        }

        private void OnNodeMouseOver(MouseEventArgs _, NodeRect rect)
        {
            if (ChartOptions!.HighlightOnHover) ActiveNode = rect.Name;
        }

        private void OnNodeMouseOut(MouseEventArgs _)
        {
            ActiveNode = null;
        }

        private async Task OnNodeClick(MouseEventArgs _, NodeRect rect)
        {
            var index = Nodes.ToList().IndexOf(Nodes.First(n => n.Name == rect.Name));

            await SetSelectedIndexAsync(index);
        }

        private void OnEdgeMouseOver(MouseEventArgs _, EdgePath edge)
        {
            if (ChartOptions!.HighlightOnHover) ActiveEdge = edge.Name;
        }

        private void OnEdgeMouseOut(MouseEventArgs _)
        {
            ActiveEdge = null;
        }

        [JSInvokable]
        public void OnElementSizeChanged(ElementSize elementSize)
        {
            if (elementSize == null || elementSize.Timestamp <= _elementSize?.Timestamp)
                return;

            _elementSize = elementSize;

            if (!MatchBoundsToSize)
                return;

            if (Math.Abs(_boundWidth - _elementSize.Width) < Epsilon &&
                Math.Abs(_boundHeight - _elementSize.Height) < Epsilon)
            {
                return;
            }

            RebuildChart();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dotNetObjectReference?.Dispose();
                _dotNetObjectReference = null;
            }
        }
    }
}
