using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities;


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

        private readonly List<SvgLegend> _legend = [];
        private readonly HashSet<string> _hiddenNodes = [];
        private readonly Dictionary<string, string> _nodeColorCache = [];

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

        private List<ChartSeries<T>> _seriesData = [];
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

            _nodeColorCache.Clear();

            _seriesData = AggregateSeriesData(ChartOptions?.AggregationOption ?? AggregationOption.None);

            Edges = EnsureUniqueEdges();
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

                var existing = Nodes.Select(n => n.Name).ToHashSet();

                Nodes.UnionWith(overrides
                     .Where(o => !existing.Contains(o.Name))
                     .Select(o => new SankeyNode(o.Name, o.Column, o.Color))
                );
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
                throw new ArgumentException($"Edge {edgeWithInvalidNode.Source} {ChartOptions!.EdgeLabelSymbol} {edgeWithInvalidNode.Target} specifies a non-existing node");
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
            _legend.Clear();
            BuildLegends();

            if (Nodes.Count == 0)
            {
                NodeRects.Clear();
                EdgePaths.Clear();
                NodeValues.Clear();
                return;
            }

            SetBounds();
            var nodes = GetAllNodesToDraw();
            var edges = GetAllEdgesToDraw(nodes);
            if (ChartOptions!.HideNodesWithNoEdges) nodes = RemoveNodesWithNoEdges(nodes, edges);

            GenerateNodeRects(nodes, out var maxColumnValue, out var boundHeightRelativeToNodeHeight);
            GenerateEdgePaths(edges, maxColumnValue, boundHeightRelativeToNodeHeight);
        }

        private SankeyNode[] GetAllNodesToDraw()
        {
            NodeValues = GetAllNodeValues();
            var nodes = Nodes.Where(n => NodeValues[n.Name] >= ChartOptions!.HideNodesSmallerThan).ToList();
            NodeValues = NodeValues.Where(kv => nodes.Any(n => n.Name == kv.Key)).ToDictionary();

            if (ChartOptions!.OrderNodesByValue) nodes = nodes.OrderByDescending(n => NodeValues[n.Name]).ToList();

            return nodes.ToArray();
        }

        private SankeyEdge<T>[] GetAllEdgesToDraw(SankeyNode[] nodes)
        {
            return Edges
                .Where(e => nodes.Any(n => n.Name == e.Source) && nodes.Any(n => n.Name == e.Target))
                .ToArray();
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

                    // Only follow an edge when it pushes the target into a deeper column, and never
                    // past the node count (the longest path in a graph of N nodes spans at most N-1
                    // edges). Without this guard, circular edge definitions (e.g. A->B, B->A) keep
                    // enqueueing with an ever-increasing column and hang the render.
                    if (targetColumn < allNodes.Count && targetColumn > nodeColumns[target])
                    {
                        nodeColumns[target] = targetColumn;
                        queue.Enqueue((target, targetColumn));
                    }
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
                AggregationOption.GroupByLabel => AggregateByLabel(),
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

        private List<ChartSeries<T>> AggregateByLabel()
        {
            var result = new List<ChartSeries<T>>();
            var visibleSeries = ChartSeries.Where(s => s.Visible).ToList();

            for (var i = 0; i < ChartLabels.Length; i++)
            {
                if (_hiddenNodes.Contains(ChartLabels[i]))
                    continue;

                var label = ChartLabels[i];
                var data = new List<(SankeyLink, T)>();

                foreach (var series in visibleSeries)
                {
                    var values = series.Data?.Values ?? [];

                    if (i < values.Count)
                    {
                        var link = new SankeyLink(label, series.Name);
                        data.Add((link, values[i]));
                    }
                }

                result.Add(new ChartSeries<T>
                {
                    Name = label,
                    Data = data.ToArray(),
                    Visible = true
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

        private HashSet<SankeyEdge<T>> EnsureUniqueEdges()
        {
            var unique = new HashSet<SankeyEdge<T>>();

            foreach (var series in _seriesData.Where(s => s.Visible))
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

        private static SankeyNode[] RemoveNodesWithNoEdges(SankeyNode[] nodes, SankeyEdge<T>[] edges)
        {
            return nodes
                .Where(n => edges.Any(e => e.Source == n.Name || e.Target == n.Name))
                .ToArray();
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

        private void GenerateNodeRects(SankeyNode[] nodes, out double maxColumnValue, out double boundHeightRelativeToNodeHeight)
        {
            NodeRects.Clear();

            var nodesPerColumn = NormaliseNodeColumnIndices(nodes)
                .GroupBy(x => x.Column)
                .OrderBy(grp => grp.Key)
                .ToArray();
            maxColumnValue = nodes
                .GroupBy(n => n.Column)
                .Select(grp => grp.Sum(n => NodeValues.GetValueOrDefault(n.Name)))
                .Max();
            var relativeNodesValuesMapping = GetNormalisedNodeValuesMapping(nodes, maxColumnValue);

            // Calculate grid sizes
            var maxRows = nodesPerColumn.Max(n => n.Count());
            var maxColumns = nodesPerColumn.Length - 1;
            var boundWidthRelativeToNodeWidth = _boundWidth - (ChartOptions!.NodeWidth * maxColumns) - (2 * HorizontalPadding);

            boundHeightRelativeToNodeHeight = _boundHeight - (ChartOptions!.MinVerticalSpacing * maxRows);

            // Draw all nodes column per column
            foreach (var column in nodesPerColumn)
            {
                var x = (column.First().Column / (double)maxColumns * boundWidthRelativeToNodeWidth) + HorizontalPadding;
                var totalRelativeColumnValue = column.Sum(n => relativeNodesValuesMapping[n]);
                var totalVerticalSpace = _boundHeight - (double.CreateSaturating(totalRelativeColumnValue) * boundHeightRelativeToNodeHeight);
                var verticalSpacing = Math.Max(totalVerticalSpace / (column.Count() + 1), ChartOptions!.MinVerticalSpacing);

                double currentY = 0;
                foreach (var node in column)
                {
                    var y = currentY + verticalSpacing;
                    var height = double.CreateSaturating(relativeNodesValuesMapping[node]) * boundHeightRelativeToNodeHeight;

                    if (!_nodeColorCache.ContainsKey(node.Name))
                        _nodeColorCache[node.Name] = GetNextHexColorForNodeRect(node);

                    NodeRects[node.Name] = new NodeRect(
                        Hash: node.GetHashCode(),
                        Name: node.Name,
                        X: x,
                        Y: y,
                        Width: ChartOptions!.NodeWidth,
                        Height: height,
                        Color: _nodeColorCache[node.Name]
                    );

                    currentY = y + height;
                }
            }
        }

        private static SankeyNode[] NormaliseNodeColumnIndices(SankeyNode[] nodes)
        {
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

        private Dictionary<SankeyNode, double> GetNormalisedNodeValuesMapping(SankeyNode[] nodes, double maxColumnValue)
        {
            var result = new Dictionary<SankeyNode, double>();
            foreach (var node in nodes)
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
                return palette[_nodeColorCache.Count % palette.Length];
            }

            return Colors.Gray.Default;
        }

        private void GenerateEdgePaths(SankeyEdge<T>[] edges, double maxColumnValue, double relativeBoundHeight)
        {
            EdgePaths.Clear();

            var index = 0;
            var edgesPerSources = edges.GroupBy(e => e.Source).ToList();
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
                        Name = $"{rectSource.Name} {ChartOptions!.EdgeLabelSymbol} {rectTarget.Name} ({edge.Weight})",
                        Source = rectSource,
                        Target = rectTarget,
                        Data = BuildSankyEdgePath(
                            sourceX: startX - 0.01, // -0.01 to prevent a visible edge when setting the edge opacity to 1
                            sourceY: startY,
                            sourceHeight: height,
                            targetX: endX + 0.01, // +0.01 to prevent a visible edge when setting the edge opacity to 1
                            targetY: endY,
                            targetHeight: height
                        ),
                        LabelXValue = rectSource.Name,
                        LabelYValue = rectTarget.Name,
                        LabelX = startX + (Math.Abs(startX - endX) / 2),
                        LabelY = startY + ((endY - startY) / 2) + (height / 2)
                    });

                    startYOffset += height;
                    rectTarget.LowestIncomingNodeY += height;
                }
            }
        }

        [SuppressMessage("ReSharper", "InlineTemporaryVariable")]
        private static string BuildSankyEdgePath(double sourceX, double sourceY, double sourceHeight, double targetX, double targetY, double targetHeight, double curvature = 0.5)
        {
            // Midpoints of source and target edges
            var sy0 = sourceY;
            var sy1 = sourceY + sourceHeight;
            var ty0 = targetY;
            var ty1 = targetY + targetHeight;

            // Control points for cubic Bezier curve
            var cx0 = sourceX + ((targetX - sourceX) * curvature);
            var cx1 = targetX - ((targetX - sourceX) * curvature);

            return $"M{ToS(sourceX)},{ToS(sy0)} " + // Top-left of source
                   $"C{ToS(cx0)},{ToS(sy0)} " + // Control point 1
                   $"{ToS(cx1)},{ToS(ty0)} " + // Control point 2
                   $"{ToS(targetX)},{ToS(ty0)} " + // Top of target
                   $"L{ToS(targetX)},{ToS(ty1)} " + // Bottom of target
                   $"C{ToS(cx1)},{ToS(ty1)} " + // Control point 2 mirrored
                   $"{ToS(cx0)},{ToS(sy1)} " + // Control point 1 mirrored
                   $"{ToS(sourceX)},{ToS(sy1)} Z"; // Bottom of source
        }

        private void BuildLegends()
        {
            for (var i = 0; i < _seriesData.Count; i++)
            {
                var name = _seriesData[i].Name;

                _legend.Add(new SvgLegend
                {
                    Index = i,
                    Labels = name,
                    Visible = ChartOptions!.AggregationOption == AggregationOption.GroupByLabel
                        ? !_hiddenNodes.Contains(ChartLabels[i])
                        : _seriesData[i].Visible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                });
            }
        }

        protected void HandleLegendVisibilityChanged(SvgLegend legend)
        {
            if (legend.Visible)
                _hiddenNodes.Remove(legend.Labels);
            else
                _hiddenNodes.Add(legend.Labels);

            _seriesData.First(x => x.Name == legend.Labels).Visible = legend.Visible;

            Edges = EnsureUniqueEdges();
            Nodes = GenerateNodesFromEdges();

            RebuildChart();
        }

        private void OnNodeMouseOver(NodeRect rect)
        {
            if (ChartOptions!.HighlightOnHover) ActiveNode = rect.Name;
        }

        private void OnNodeMouseOut()
        {
            ActiveNode = null;
        }

        private async Task OnNodeClick(NodeRect rect)
        {
            var index = Nodes.ToList().IndexOf(Nodes.First(n => n.Name == rect.Name));

            await SetSelectedIndexAsync(index);
        }

        private void OnEdgeMouseOver(EdgePath edge)
        {
            if (ChartOptions!.HighlightOnHover) ActiveEdge = edge.Name;
        }

        private void OnEdgeMouseOut()
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
