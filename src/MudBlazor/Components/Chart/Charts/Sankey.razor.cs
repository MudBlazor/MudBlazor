using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays data as nodes connected by weighted edges.
    /// </summary>
    partial class Sankey<T> : MudChartBase<T, SankeyChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        private record NodeRect(int Hash, string Name, double X, double Y, double Width, double Height, string Color)
        {
            public double LowestIncomingNodeY { get; set; } = Y;
        }

        private record EdgePath(string Name, NodeRect Source, NodeRect Target, string D, double CenterX, double CenterY);

        private const double BoundWidth = 650;
        private const double BoundHeight = 350;
        private const double HorizontalPadding = 10;

        private HashSet<SankeyNode> Nodes { get; set; } = [];
        private HashSet<SankeyEdge<T>> Edges { get; set; } = [];
        private Dictionary<string, NodeRect> NodeRects { get; } = [];
        private Dictionary<string, double> NodeValues { get; set; } = [];
        private List<EdgePath> EdgePaths { get; } = [];
        private string? ActiveNode { get; set; }
        private string? ActiveEdge { get; set; }

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart<T>? MudChartParent { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Edges = EnsureUniqueEdges();
            Nodes = GenerateNodesFromEdges();

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

        public override void RebuildChart()
        {
            NodeValues = GetAllNodeValues();
            var (maxColumnValue, relativeBoundHeight) = GenerateNodeRects();
            GenerateEdgePaths(maxColumnValue, relativeBoundHeight);
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

        private HashSet<SankeyEdge<T>> EnsureUniqueEdges()
        {
            var unique = new HashSet<SankeyEdge<T>>();

            foreach (var series in ChartSeries)
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
            var boundHeightRelativeToNodeHeight = BoundHeight - ChartOptions!.MinVerticalSpacing * maxRows;
            var boundWidthRelativeToNodeWidth = BoundWidth - ChartOptions!.NodeWidth * maxColumns - 2 * HorizontalPadding;

            // Draw all nodes column per column
            foreach (var column in nodesPerColumn)
            {
                var x = column.First().Column / (double)maxColumns * boundWidthRelativeToNodeWidth + HorizontalPadding;
                var totalRelativeColumnValue = column.Sum(n => relativeNodesValuesMapping[n]); //column.Aggregate(T.Zero, (sum, n) => sum + relativeNodesValuesMapping[n]);
                var totalVerticalSpace = BoundHeight - double.CreateSaturating(totalRelativeColumnValue) * boundHeightRelativeToNodeHeight;
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
            //if (node.Color is not null)
            //{
            //    return node.Color.ToString(MudColorOutputFormats.HexA);
            //}

            if (MudChartParent?.ChartOptions!.ChartPalette is { Length: > 0 } palette)
            {
                return palette[NodeRects.Count % palette.Length];
            }

            return Colors.Gray.Default;
        }

        private void GenerateEdgePaths(double maxColumnValue, double relativeBoundHeight)
        {
            EdgePaths.Clear();

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

                    EdgePaths.Add(new EdgePath(
                        Name: $"{rectSource.Name} -> {rectTarget.Name} ({edge.Weight})",
                        Source: rectSource,
                        Target: rectTarget,
                        D: BuildSankyEdgePath(
                            sourceX: startX - 0.1, // -0.1 to prevent a visible edge when setting the edge opacity to 1
                            sourceY: startY,
                            sourceHeight: height,
                            targetX: endX + 0.1, // +0.1 to prevent a visible edge when setting the edge opacity to 1
                            targetY: endY,
                            targetHeight: height
                        ),
                        CenterX: startX + Math.Abs(startX - endX) / 2,
                        CenterY: startY + Math.Abs(startY - (endY + height)) / 2
                    ));

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
            const double curvature = 0.5;
            var cx0 = sourceX + (targetX - sourceX) * curvature;
            var cx1 = targetX - (targetX - sourceX) * curvature;

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

        private void OnNodeClick(MouseEventArgs _, NodeRect rect)
        {
            SelectedIndex = Nodes.ToList().IndexOf(Nodes.First(n => n.Name == rect.Name));
        }

        private void OnEdgeMouseOver(MouseEventArgs _, EdgePath edge)
        {
            if (ChartOptions!.HighlightOnHover) ActiveEdge = edge.Name;
        }

        private void OnEdgeMouseOut(MouseEventArgs _)
        {
            ActiveEdge = null;
        }
    }
}
