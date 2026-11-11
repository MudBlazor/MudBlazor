using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace MudBlazor.Charts;

/// <summary>
/// Renders the floating tooltip shown when hovering data points in MudBlazor's charts.
/// </summary>
public partial class ChartTooltip : ComponentBase
{
    internal sealed record BBox(double X = 0, double Y = 0, double Width = 0, double Height = 0);

    private const double TriangleWidth = 16;
    private const double TriangleHeight = 8;
    private const double CornerRadius = 4;

    private double TriangleStrokeWidth => StrokeWidth + 1;
    private bool HasSubtitle => !string.IsNullOrWhiteSpace(Subtitle);

    private ElementReference? _text;
    private BBox _backgroundBBox = new();
    private BBox _textBbox = new();
    private string _triangleBackgroundPoints = "";
    private string _triangleBorderPoints = "";

    [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The title of the tooltip.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The subtitle of the tooltip.
    /// </summary>
    /// <remarks>
    /// When empty, the subtitle is not displayed.
    /// </remarks>
    [Parameter]
    public string Subtitle { get; set; } = string.Empty;

    /// <summary>
    /// The X coordinate of the tooltip anchor.
    /// </summary>
    [Parameter, EditorRequired]
    public double X { get; set; }

    /// <summary>
    /// The Y coordinate of the tooltip anchor.
    /// </summary>
    [Parameter, EditorRequired]
    public double Y { get; set; }

    /// <summary>
    /// The color of the tooltip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"darkgrey"</c>.
    /// </remarks>
    [Parameter]
    public string Color { get; set; } = "darkgrey";

    /// <summary>
    /// The font size of the <see cref="Title"/> and <see cref="Subtitle"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"12px"</c>.
    /// </remarks>
    [Parameter]
    public string FontSize { get; set; } = "12px";

    /// <summary>
    /// The padding size of the tooltip background in px.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>5</c>.
    /// </remarks>
    [Parameter]
    public int PaddingSize { get; set; } = 5;

    /// <summary>
    /// The color of the <see cref="Title"/> and <see cref="Subtitle"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"white"</c>.
    /// </remarks>
    [Parameter]
    public string FontColor { get; set; } = "white";

    /// <summary>
    /// The width of the border.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>.
    /// </remarks>
    [Parameter]
    public int StrokeWidth { get; set; } = 1;

    /// <summary>
    /// Whether to show the triangle pointing at the anchor.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    public bool ShowTriangle { get; set; } = true;

    /// <summary>
    /// How to align the tooltip in respect to the given x coordinates.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>ChartTooltipAnchorPositionX.Center</c>.
    /// </remarks>
    [Parameter]
    public ChartTooltipAnchorPositionX AnchorPositionX { get; set; } = ChartTooltipAnchorPositionX.Center;

    /// <summary>
    /// The room the chart has around the point this tooltip describes.
    /// </summary>
    /// <remarks>
    /// Cascaded by the chart rather than passed, so a tooltip written into a chart's <c>TooltipTemplate</c> moves below its point too.
    /// </remarks>
    [CascadingParameter]
    internal ChartTooltipRoom? Room { get; set; }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(BBox))]
    public ChartTooltip() { }

    private double _previousX;
    private double _previousY;
    private string? _previousFontSize;
    private string? _previousTitle;
    private string? _previousSubtitle;
    private ChartTooltipRoom? _previousRoom;
    private bool _isMeasured;
    private int _measurementId;

    // The box is sized from the text that the browser measures after the first render.
    // Until then the text would be shown at the origin of the chart without its background.
    // Later measurements keep the tooltip visible, so labels that update don't blink.
    private string? Visibility => _isMeasured ? null : "hidden";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender || FontSize != _previousFontSize || Title != _previousTitle || Subtitle != _previousSubtitle || !_previousX.Equals(X) || !_previousY.Equals(Y) || Room != _previousRoom)
        {
            await RecalculateBoxWidthAsync();
        }
    }

    private async Task RecalculateBoxWidthAsync()
    {
        _previousX = X;
        _previousY = Y;
        _previousTitle = Title;
        _previousSubtitle = Subtitle;
        _previousFontSize = FontSize;
        _previousRoom = Room;

        var measurementId = ++_measurementId;
        var textBBox = await JsRuntime.InvokeAsync<BBox>("mudGetSvgBBox", _text);

        // A newer measurement started because the text or position changed while this one was running.
        // Its box belongs to the older text, so only the latest measurement is applied.
        if (measurementId != _measurementId)
        {
            return;
        }

        var textWidth = textBBox?.Width ?? 0;
        var textHeight = textBBox?.Height ?? 0;

        var xText = AnchorPositionX switch
        {
            ChartTooltipAnchorPositionX.Start => X + (textWidth / 2) + PaddingSize,
            ChartTooltipAnchorPositionX.Center => X,
            ChartTooltipAnchorPositionX.End => X - (textWidth / 2) - PaddingSize,
            _ => throw new ArgumentException($"Unknown relative position {AnchorPositionX}")
        };
        if (ShowTriangle) textWidth = Math.Max(textWidth, TriangleWidth);
        var triangleOffsetY = ShowTriangle ? (TriangleHeight * 2) + TriangleStrokeWidth : 0;
        _textBbox = new BBox(
            X: xText,
            Y: Y + (textHeight / 2) - triangleOffsetY,
            Width: textWidth,
            Height: textHeight
        );

        var backgroundWidth = textWidth + (PaddingSize * 2);
        var xBackground = AnchorPositionX switch
        {
            ChartTooltipAnchorPositionX.Start => X,
            ChartTooltipAnchorPositionX.Center => X - (backgroundWidth / 2),
            ChartTooltipAnchorPositionX.End => X - backgroundWidth,
            _ => throw new ArgumentException($"Unknown relative position {AnchorPositionX}")
        };
        var backgroundHeight = textHeight + (PaddingSize * 2);
        _backgroundBBox = new BBox(
            X: xBackground,
            Y: Y - (backgroundHeight / 2 * (HasSubtitle ? 2.1 : 1)) - triangleOffsetY,
            Width: backgroundWidth,
            Height: backgroundHeight * (HasSubtitle ? 1.5 : 1)
        );

        // A tooltip that would run off the top of the chart is drawn below its point instead, as the same shape upside down.
        var room = Room;
        var reachAbove = Y - _backgroundBBox.Y;
        var flip = room is not null && reachAbove > room.Above && reachAbove <= room.Below;
        var anchorY = flip ? Y + room!.FlipOffset : Y;

        if (flip)
        {
            var top = anchorY + (Y - (_backgroundBBox.Y + _backgroundBBox.Height));
            _textBbox = _textBbox with { Y = _textBbox.Y + top - _backgroundBBox.Y };
            _backgroundBBox = _backgroundBBox with { Y = top };
        }

        // A tooltip that would run past the left or right edge of the chart slides back in, but only as far as the triangle still meets the box.
        if (room is not null)
        {
            var shift = SlideInside(_backgroundBBox.X, _backgroundBBox.Width, X - room.Left, X + room.Right);
            _textBbox = _textBbox with { X = _textBbox.X + shift };
            _backgroundBBox = _backgroundBBox with { X = _backgroundBBox.X + shift };
        }

        // Distances from the anchor run up toward the box, or down once the tooltip is flipped.
        double AwayFromPoint(double distance) => flip ? anchorY + distance : anchorY - distance;

        if (ShowTriangle)
        {
            _triangleBorderPoints = $"{ToS(X - (TriangleWidth / 2))},{ToS(AwayFromPoint(TriangleHeight))} " + // Left
                                    $"{ToS(X + (TriangleWidth / 2))},{ToS(AwayFromPoint(TriangleHeight))} " + // Right
                                    $"{ToS(X)},{ToS(AwayFromPoint(TriangleStrokeWidth))}"; // Tip
            _triangleBackgroundPoints = $"{ToS(X - (TriangleWidth / 2) - TriangleStrokeWidth)},{ToS(AwayFromPoint(TriangleHeight + TriangleStrokeWidth))} " + // Left
                                        $"{ToS(X + (TriangleWidth / 2) + TriangleStrokeWidth)},{ToS(AwayFromPoint(TriangleHeight + TriangleStrokeWidth))} " + // Right
                                        $"{ToS(X)},{ToS(AwayFromPoint(TriangleStrokeWidth))}"; // Tip
        }

        _isMeasured = true;
        StateHasChanged();
    }

    private double SlideInside(double left, double width, double leftEdge, double rightEdge)
    {
        // How far the box can move before the triangle under it would hang past the rounded corner.
        var keep = ShowTriangle ? (TriangleWidth / 2) + TriangleStrokeWidth + CornerRadius : 0;

        if (left < leftEdge)
        {
            return Math.Max(0, Math.Min(leftEdge, X - keep) - left);
        }

        if (left + width > rightEdge)
        {
            return Math.Min(0, Math.Max(rightEdge - width, X + keep - width) - left);
        }

        return 0;
    }
}

/// <summary>
/// The horizontal anchor position of a chart tooltip relative to its data point.
/// </summary>
public enum ChartTooltipAnchorPositionX
{
    Start,
    Center,
    End
}

/// <summary>
/// The room a chart has around a hovered point, which a <see cref="ChartTooltip"/> uses to move below the point when it doesn't fit above it.
/// </summary>
/// <param name="Above">The distance from the tooltip's anchor up to the top of the chart.</param>
/// <param name="Below">The distance from the flipped anchor down to the bottom of the chart.</param>
/// <param name="FlipOffset">How far below the usual anchor the flipped one sits, which clears the point itself.</param>
/// <param name="Left">The distance from the anchor to the left edge of the chart.</param>
/// <param name="Right">The distance from the anchor to the right edge of the chart.</param>
internal sealed record ChartTooltipRoom(double Above, double Below, double FlipOffset, double Left, double Right);
