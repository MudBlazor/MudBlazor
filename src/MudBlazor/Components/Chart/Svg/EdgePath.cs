// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Represents the path of a link (edge) between two nodes in a Sankey chart.
/// </summary>
/// <remarks>
/// An <see cref="EdgePath"/> defines the SVG path data and associated metadata
/// for rendering a visual connection between a <see cref="Source"/> node
/// and a <see cref="Target"/> node. It includes the computed Bézier curve
/// or straight path defined by <see cref="SvgPath.Data"/>, and geometric properties
/// such as the center point of the edge for positioning labels or tooltips.
/// </remarks>
public sealed class EdgePath : SvgPath
{
    /// <summary>
    /// The name of the edge, typically corresponding to the 
    /// label or key identifying the link between the source and target nodes.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The source node of this edge.
    /// </summary>
    /// <remarks>
    /// Represents the originating <see cref="NodeRect"/> (visual node rectangle)
    /// from which the flow begins in the Sankey diagram.
    /// </remarks>
    public required NodeRect Source { get; set; }

    /// <summary>
    /// The target node of this edge.
    /// </summary>
    /// <remarks>
    /// Represents the destination <see cref="NodeRect"/> (visual node rectangle)
    /// to which the flow connects in the Sankey diagram.
    /// </remarks>
    public required NodeRect Target { get; set; }
}

/// <summary>
/// Represents a rectangular node in a Sankey chart, including its position,
/// dimensions, color.
/// </summary>
/// <remarks>
/// A <see cref="NodeRect"/> defines the bounding box of a node (or block)
/// that acts as a source or target within the Sankey diagram. It contains
/// both layout data (position and size) and presentation data (color and label).
/// </remarks>
/// <param name="Hash">
/// A unique identifier or hash value for this node, used to ensure consistent mapping between nodes and edges.
/// </param>
/// <param name="Name">
/// The display name or label of the node, typically representing a category or entity
/// in the Sankey flow.
/// </param>
/// <param name="X">
/// The X-coordinate of the node’s top-left corner in the SVG coordinate space.
/// </param>
/// <param name="Y">
/// The Y-coordinate of the node’s top-left corner in the SVG coordinate space.
/// </param>
/// <param name="Width">
/// The width of the node’s rectangle, usually proportional to the node’s total inflow or outflow.
/// </param>
/// <param name="Height">
/// The height of the node’s rectangle, usually proportional to the flow magnitude it represents.
/// </param>
/// <param name="Color">
/// The fill color used to visually represent the node in the chart.
/// </param>
public sealed record NodeRect(int Hash, string Name, double X, double Y, double Width, double Height, string Color)
{
    /// <summary>
    /// The lowest Y-coordinate among all incoming edges connecting to this node.
    /// </summary>
    /// <remarks>
    /// Used during layout calculation to position incoming flows without overlap
    /// and to align edges smoothly. Defaults to the node’s initial <see cref="Y"/> value.
    /// </remarks>
    public double LowestIncomingNodeY { get; set; } = Y;
}

