using System.Globalization;
using Microsoft.AspNetCore.Components;
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

        private const double NodeWidth = 10;

        private const double MinVerticalSpacing = 10;

        private const double EdgeOpacity = 0.5;

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private record NodeRect(int Hash, double X, double Y, double Width, double Height, string Color)
        {
            public double LowestIncomingNodeY { get; set; } = Y;
        }
        private record EdgePath(string Source, string Target, string D);

        private Dictionary<string, NodeRect> _nodeRects { get; } = [];
        private List<EdgePath> _edgePaths { get; } = [];

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
                var (maxColumnValue, relativeBoundHeight) = GenerateNodeRects();
                GenerateEdgePaths(maxColumnValue, relativeBoundHeight);
            }
        }

        private (double MaxNodeValue, double RealtiveBoundHeight) GenerateNodeRects()
        {
            _nodeRects.Clear();

            var nodesPerColumn = NormaliseNodeColumnIndices()
                .GroupBy(x => x.Column)
                .OrderBy(grp => grp.Key)
                .ToArray();
            var allNodeValues = GetAllNodeValues();
            var maxColumnValue = Nodes
                .GroupBy(n => n.Column)
                .Select(grp => grp.Sum(n => allNodeValues[n.Name]))
                .Max();
            var relativeNodesValuesMapping = GetNormalisedNodeValuesMapping(allNodeValues, maxColumnValue);

            // Calculate grid sizes
            var maxRows = nodesPerColumn.Max(n => n.Count());
            var maxColumns = nodesPerColumn.Length - 1;
            var boundHeightRelativeToNodeHeight = BoundHeight - MinVerticalSpacing * maxRows;
            var boundWidthRelativeToNodeWidth = BoundWidth - NodeWidth * maxColumns;

            // Draw all nodes column per column
            foreach (var column in nodesPerColumn)
            {
                var x = column.First().Column / (double)maxColumns * boundWidthRelativeToNodeWidth;
                var totalRelativeColumnValue = column.Sum(n => relativeNodesValuesMapping[n]);
                var totalVerticalSpace = BoundHeight - totalRelativeColumnValue * boundHeightRelativeToNodeHeight;
                var verticalSpacing = Math.Max(totalVerticalSpace / (column.Count() + 1), MinVerticalSpacing);

                double currentY = 0;
                foreach (var node in column)
                {
                    var y = currentY + verticalSpacing;
                    var height = relativeNodesValuesMapping[node] * boundHeightRelativeToNodeHeight;

                    _nodeRects[node.Name] = new NodeRect(
                        Hash: node.GetHashCode(),
                        X: x,
                        Y: y,
                        Width: NodeWidth,
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

        private Dictionary<SankeyChartNode, double> GetNormalisedNodeValuesMapping(Dictionary<string, double> nodeValues, double maxColumnValue)
        {
            var result = new Dictionary<SankeyChartNode, double>();
            foreach (var node in Nodes)
            {
                result[node] = nodeValues[node.Name] / maxColumnValue;
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
                        Source: edge.Source,
                        Target: edge.Target,
                        D: BuildSankyEdgePath(
                            sourceX: startX,
                            sourceY: startY,
                            sourceHeight: height,
                            targetX: endX,
                            targetY: endY,
                            targetHeight: height
                        )
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

            return $"M{sourceX.ToString(Culture)},{sy0.ToString(Culture)} " + // Top-left of source
                   $"C{cx0.ToString(Culture)},{sy0.ToString(Culture)} " + // Control point 1
                   $"{cx1.ToString(Culture)},{ty0.ToString(Culture)} " + // Control point 2
                   $"{targetX.ToString(Culture)},{ty0.ToString(Culture)} " + // Top of target
                   $"L{targetX.ToString(Culture)},{ty1.ToString(Culture)} " + // Bottom of target
                   $"C{cx1.ToString(Culture)},{ty1.ToString(Culture)} " + // Control point 2 mirrored
                   $"{cx0.ToString(Culture)},{sy1.ToString(Culture)} " + // Control point 1 mirrored
                   $"{sourceX.ToString(Culture)},{sy1.ToString(Culture)} Z"; // Bottom of source
        }
    }
}
