using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace MudBlazor.Charts;

public partial class ChartTooltip : ComponentBase
{
    private sealed record BBox(double X = 0, double Y = 0, double Width = 0, double Height = 0);

    private const double TriangleWidth = 16;
    private const double TriangleHeight = 8;

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
    /// Defaults to <c>"unset"</c>.
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
    /// Whether to show the triangle pointing down.
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
    /// Defaults to <c>ChartTooltipRelativePositionX.Center</c>.
    /// </remarks>
    [Parameter]
    public ChartTooltipAnchorPositionX AnchorPositionX { get; set; } = ChartTooltipAnchorPositionX.Center;

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(BBox))]
    public ChartTooltip() { }

    private double _previousX;
    private double _previousY;
    private string? _previousFontSize;
    private string? _previousTitle;
    private string? _previousSubtitle;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender || FontSize != _previousFontSize || Title != _previousTitle || Subtitle != _previousSubtitle || _previousX != X || _previousY != Y)
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

        var textBBox = await JsRuntime.InvokeAsync<BBox>("mudGetSvgBBox", _text);
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

        if (ShowTriangle)
        {
            _triangleBorderPoints = $"{ToS(X - (TriangleWidth / 2))},{ToS(Y - TriangleHeight)} " + // Left
                                    $"{ToS(X + (TriangleWidth / 2))},{ToS(Y - TriangleHeight)} " + // Right
                                    $"{ToS(X)},{ToS(Y - TriangleStrokeWidth)}"; // Bottom
            _triangleBackgroundPoints = $"{ToS(X - (TriangleWidth / 2) - TriangleStrokeWidth)},{ToS(Y - TriangleHeight - TriangleStrokeWidth)} " + // Left
                                        $"{ToS(X + (TriangleWidth / 2) + TriangleStrokeWidth)},{ToS(Y - TriangleHeight - TriangleStrokeWidth)} " + // Right
                                        $"{ToS(X)},{ToS(Y - TriangleStrokeWidth)}"; // Bottom
        }

        StateHasChanged();
    }
}

public enum ChartTooltipAnchorPositionX
{
    Start,
    Center,
    End
}
