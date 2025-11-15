#nullable enable

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Two panels which are resizeable.
/// </summary>
public partial class MudSplitPanel : MudComponentBase
{
    private string Classname => new CssBuilder("mud-split-panel")
        .AddClass("flex-column", Horizontal)
        .AddClass("absolute", UseAsOverlay)
        .AddClass(Class)
        .Build();

    private string ClassnameFirstPanel => new CssBuilder("child-panel")
        .AddClass($"mud-elevation-{Elevation}", Elevation != 0 && FirstPanel != null)
        .AddClass("transparent", FirstPanel == null)
        .AddClass($"pa-{Padding}", Padding != 0)
        .AddClass("rounded", Rounded)
        .AddClass(ClassFirstPanel)
        .Build();

    private string StylenameFirstPanel => new StyleBuilder()
        .AddStyle($"background-color: {BackgroundColor}", FirstPanel != null)
        .AddStyle(StyleFirstPanel)
        .Build();

    private string ClassnameSecondPanel => new CssBuilder("child-panel")
        .AddClass($"mud-elevation-{Elevation}", Elevation != 0 && SecondPanel != null)
        .AddClass("transparent", SecondPanel == null)
        .AddClass($"pa-{Padding}", Padding != 0)
        .AddClass("rounded", Rounded)
        .AddClass(ClassSecondPanel)
        .Build();

    private string StylenameSecondPanel => new StyleBuilder()
        .AddStyle($"background-color: {BackgroundColor}", SecondPanel != null)
        .AddStyle(StyleFirstPanel)
        .Build();

    private string ClassnameDivider => new CssBuilder("divider")
        .AddClass("horizontal", Horizontal)
        .AddClass(ClassDivider)
        .Build();

    /// <summary>
    /// The CSS styles applied to the first panel.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public string? StyleFirstPanel { get; set; }

    /// <summary>
    /// The CSS styles applied to the second panel.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public string? StyleSecondPanel { get; set; }

    /// <summary>
    /// The CSS classes applied to the first panel.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public string? ClassFirstPanel { get; set; }

    /// <summary>
    /// The CSS classes applied to the second panel.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public string? ClassSecondPanel { get; set; }

    /// <summary>
    /// The CSS styles applied to the divider.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public string? StyleDivider { get; set; }

    /// <summary>
    /// The CSS classes applied to the divider.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public string? ClassDivider { get; set; }

    /// <summary>
    /// Whether the panels should be divided horizontally instead of vertically. 
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool Horizontal { get; set; }

    /// <summary>
    /// Positions this panel absolute and above other content.
    /// Note that you have to set <c>position: relative</c> on the parent element.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool UseAsOverlay { get; set; }

    /// <summary>
    /// Whether to use rounded corners for the panels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool Rounded { get; set; }

    /// <summary>
    /// The size of the drop shadow.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0</c>.
    /// A higher number creates a heavier drop shadow.
    /// Set to <c>0</c> to disable the drop shadow.
    /// </remarks>
    [Parameter]
    public int Elevation { get; set; } = 0;

    /// <summary>
    /// Sets the initial height or width of the first panel in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public int? FirstPanelInitialSize { get; set; }

    /// <summary>
    /// The height and width in pixel each panel can't be made smaller than.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>50</c>.
    /// </remarks>
    [Parameter]
    public int MinPanelSize { get; set; } = 50;

    /// <summary>
    /// The padding of the pannels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0</c>.
    /// </remarks>
    [Parameter]
    public int Padding { get; set; }

    /// <summary>
    /// The padding of the pannels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0</c>.
    /// </remarks>
    [Parameter]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// The contents of the first i.e. left/upper panel.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public RenderFragment? FirstPanel { get; set; }

    /// <summary>
    /// The contents of the second i.e. right/lower panel.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    public RenderFragment? SecondPanel { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private readonly string _containerId = Guid.NewGuid().ToString();
    private bool _isRendered;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _isRendered = true;
            await JsRuntime.InvokeVoidAsync("mudSplitPanel.build", _containerId, Horizontal, MinPanelSize, FirstPanelInitialSize);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (_isRendered) await JsRuntime.InvokeVoidAsync("mudSplitPanel_update", _containerId, Horizontal, MinPanelSize);
    }
}
