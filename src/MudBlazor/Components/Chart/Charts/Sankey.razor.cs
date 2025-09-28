using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays data as nodes connected by weighted edges.
    /// </summary>
    partial class Sankey : MudCategoryChartBase
    {
        private const double BoundWidth = 650;

        private const double BoundHeight = 350;

        private record NodeRect(int Hash, string Name, double X, double Y, double Width, double Height, string Color)
        {
            public double LowestIncomingNodeY { get; set; } = Y;
        }
        private record EdgePath(NodeRect Source, NodeRect Target, string D, double Value, double CenterX, double CenterY);

        private Dictionary<string, NodeRect> _nodeRects { get; } = [];
        
        private List<EdgePath> _edgePaths { get; } = [];
        
        private Dictionary<string, double> _nodeValues { get; set; } = [];

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // Assert input data
            var nodeGroups = Nodes.GroupBy(e => e.Name).ToList();
            if (nodeGroups.Any(grp => grp.Count() > 1))
            {
                throw new ArgumentException("All nodes must have unique names");
            }

            if (MudChartParent != null)
            {
                _nodeValues = GetAllNodeValues();
                var (maxColumnValue, relativeBoundHeight) = GenerateNodeRects();
                GenerateEdgePaths(maxColumnValue, relativeBoundHeight);
            }
        }
        
        private Dictionary<string, double> GetAllNodeValues()
        {
            var nodeValues = Edges
                .GroupBy(e => e.Target)
                .ToDictionary(grp => grp.Key, grp => grp.Sum(e => e.Value));
            Edges.Where(e => !nodeValues.ContainsKey(e.Source))
                .GroupBy(e => e.Source)
                .ToList()
                .ForEach(grp => nodeValues[grp.Key] = grp.Sum(e => e.Value));

            return nodeValues;
        }
        
        private (double MaxNodeValue, double RealtiveBoundHeight) GenerateNodeRects()
        {
            _nodeRects.Clear();

            var nodesPerColumn = NormaliseNodeColumnIndices()
                .GroupBy(x => x.Column)
                .OrderBy(grp => grp.Key)
                .ToArray();
            var maxColumnValue = Nodes
                .GroupBy(n => n.Column)
                .Select(grp => grp.Sum(n => _nodeValues[n.Name]))
                .Max();
            var relativeNodesValuesMapping = GetNormalisedNodeValuesMapping(maxColumnValue);

            // Calculate grid sizes
            var maxRows = nodesPerColumn.Max(n => n.Count());
            var maxColumns = nodesPerColumn.Length - 1;
            var boundHeightRelativeToNodeHeight = BoundHeight - NodeChartOptions.MinVerticalSpacing * maxRows;
            var boundWidthRelativeToNodeWidth = BoundWidth - NodeChartOptions.NodeWidth * maxColumns;

            // Draw all nodes column per column
            foreach (var column in nodesPerColumn)
            {
                var x = column.First().Column / (double)maxColumns * boundWidthRelativeToNodeWidth;
                var totalRelativeColumnValue = column.Sum(n => relativeNodesValuesMapping[n]);
                var totalVerticalSpace = BoundHeight - totalRelativeColumnValue * boundHeightRelativeToNodeHeight;
                var verticalSpacing = Math.Max(totalVerticalSpace / (column.Count() + 1), NodeChartOptions.MinVerticalSpacing);

                double currentY = 0;
                foreach (var node in column)
                {
                    var y = currentY + verticalSpacing;
                    var height = relativeNodesValuesMapping[node] * boundHeightRelativeToNodeHeight;

                    _nodeRects[node.Name] = new NodeRect(
                        Hash: node.GetHashCode(),
                        Name: node.Name,
                        X: x,
                        Y: y,
                        Width: NodeChartOptions.NodeWidth,
                        Height: height,
                        Color: GetNextHexColorForNodeRect(node)
                    );

                    currentY = y + height;
                }
            }

            return (maxColumnValue, boundHeightRelativeToNodeHeight);
        }

        private SankeyChartNode[] NormaliseNodeColumnIndices()
        {
            var nodes = Nodes.ToArray();

            // Normalise column indices
            var columnMap = nodes
                .Select(n => n.Column)
                .Distinct()
                .OrderBy(c => c)
                .Select((c, index) => new { Old = c, New = index })
                .ToDictionary(x => x.Old, x => x.New);
            Array.ForEach(nodes, n => n.Column = columnMap[n.Column]);

            return nodes;
        }
        
        private Dictionary<SankeyChartNode, double> GetNormalisedNodeValuesMapping(double maxColumnValue)
        {
            var result = new Dictionary<SankeyChartNode, double>();
            foreach (var node in Nodes)
            {
                result[node] = _nodeValues[node.Name] / maxColumnValue;
            }

            return result;
        }

        private string GetNextHexColorForNodeRect(SankeyChartNode node)
        {
            return node.Color?.ToString(MudColorOutputFormats.HexA)
                   ?? MudChartParent?.ChartOptions.ChartPalette.GetValue(_nodeRects.Count % MudChartParent.ChartOptions.ChartPalette.Length)!.ToString()!;
        }

        private void GenerateEdgePaths(double maxColumnValue, double relativeBoundHeight)
        {
            _edgePaths.Clear();

            var edgesPerSources = Edges.GroupBy(e => e.Source).ToList();
            foreach (var sourceGrp in edgesPerSources)
            {
                if (!_nodeRects.TryGetValue(sourceGrp.Key, out var rectSource)) continue;

                double startYOffset = 0;
                foreach (var edge in sourceGrp)
                {
                    if (!_nodeRects.TryGetValue(edge.Target, out var rectTarget)) continue;

                    var startX = rectSource.X + rectSource.Width;
                    var startY = rectSource.Y + startYOffset;
                    var endX = rectTarget.X;
                    var endY = rectTarget.LowestIncomingNodeY;
                    var height = edge.Value / maxColumnValue * relativeBoundHeight;

                    _edgePaths.Add(new EdgePath(
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
                        Value: edge.Value,
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

        private void OnNodeMouseOver(MouseEventArgs _, NodeRect rect) { }

        private void OnNodeMouseOut(MouseEventArgs _) { }

        private void OnNodeClick(MouseEventArgs _, NodeRect rect) { }

        private void OnEdgeMouseOver(MouseEventArgs _, EdgePath path) { }

        private void OnEdgeMouseOut(MouseEventArgs _) { }
    }
}
