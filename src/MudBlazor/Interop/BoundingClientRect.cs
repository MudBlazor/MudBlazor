namespace MudBlazor.Interop;
#nullable enable

/// <summary>
/// Represents the bounding rectangle of an element.
/// </summary>
public class BoundingClientRect
{
    /// <summary>
    /// The vertical offset to the top edge.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// The horizontal offset relative to the left edge.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// The vertical offset relative to the bottom edge.
    /// </summary>
    /// <returns>
    /// <see cref="Top"/> + <see cref="Height"/>
    /// </returns>
    public double Bottom => Top + Height;

    /// <summary>
    /// The horizontal offset to the right edge.
    /// </summary>
    /// <returns>
    /// <see cref="Left"/> + <see cref="Width"/>
    /// </returns>
    public double Right => Left + Width;

    /// <summary>
    /// The width of this rectangle.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// The height of this rectangle.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Width of the viewport.
    /// </summary>
    public double WindowWidth { get; set; }

    /// <summary>
    /// Height of the viewport.
    /// </summary>
    public double WindowHeight { get; set; }

    /// <summary>
    /// The horizontal scrolled offset.
    /// </summary>
    public double ScrollX { get; set; }

    /// <summary>
    /// The vertical scrolled offset.
    /// </summary>
    public double ScrollY { get; set; }

    /// <summary>
    /// The horizontal offset including the horizontal scroll.
    /// </summary>
    /// <returns>
    /// <see cref="Left"/> + <see cref="ScrollX"/>
    /// </returns>
    public double AbsoluteLeft => Left + ScrollX;

    /// <summary>
    /// The vertical offset including the vertical scroll.
    /// </summary>
    /// <returns>
    /// <see cref="Top"/> + <see cref="ScrollY"/>
    /// </returns>
    public double AbsoluteTop => Top + ScrollY;

    /// <summary>
    /// The horizontal offset from the right edge including the horizontal scroll.
    /// </summary>
    /// <returns>
    /// <see cref="Right"/> + <see cref="ScrollX"/>
    /// </returns>
    public double AbsoluteRight => Right + ScrollX;

    /// <summary>
    /// The vertical offset from the bottom edge including the vertical scroll.
    /// </summary>
    /// <returns>
    /// <see cref="Bottom"/> + <see cref="ScrollY"/>
    /// </returns>
    public double AbsoluteBottom => Bottom + ScrollY;

    /// <summary>
    /// Whether the rect is outside the viewport on the bottom side.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <see cref="Bottom"/> &gt; <see cref="WindowHeight"/>
    /// </returns>
    public bool IsOutsideBottom => Bottom > WindowHeight;

    /// <summary>
    /// Whether the rect is outside the viewport on the left side.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <see cref="Left"/> &lt; <c>0</c>
    /// </returns>
    public bool IsOutsideLeft => Left < 0;

    /// <summary>
    /// Whether the rect is outside the viewport on the top side.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <see cref="Top"/> &lt; <c>0</c>
    /// </returns>
    public bool IsOutsideTop => Top < 0;

    /// <summary>
    /// Whether the rect is outside the viewport on the right side.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <see cref="Right"/> &gt; <see cref="WindowWidth"/>
    /// </returns>
    public bool IsOutsideRight => Right > WindowWidth;

    /// <summary>
    /// Creates a shallow copy of the current <see cref="BoundingClientRect"/> instance.
    /// </summary>
    /// <returns>A new <see cref="BoundingClientRect"/> instance</returns>
    public BoundingClientRect Clone()
    {
        return new BoundingClientRect
        {
            Top = Top,
            Left = Left,
            Width = Width,
            Height = Height,
            WindowWidth = WindowWidth,
            WindowHeight = WindowHeight,
            ScrollX = ScrollX,
            ScrollY = ScrollY
        };
    }
}
