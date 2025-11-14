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
        .AddClass("transparent", TransparentBackground || FirstPanel == null)
        .AddClass(ClassFirstPanel)
        .Build();

    private string ClassnameSecondPanel => new CssBuilder("child-panel")
        .AddClass("transparent", TransparentBackground || SecondPanel == null)
        .AddClass(ClassSecondPanel)
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
    /// Removes the background color of both panels if set to true.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool TransparentBackground { get; set; }

    /// <summary>
    /// Positions this panel absolute and above other content.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool UseAsOverlay { get; set; }

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

    private static readonly string ContainerId = Guid.NewGuid().ToString();
    private bool _isRendered;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await Task.Delay(100); // TODO: Fix
            _isRendered = true;
            await JsRuntime.InvokeVoidAsync("mudSplitPanel.startListening", ContainerId, Horizontal);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (_isRendered) await JsRuntime.InvokeVoidAsync("mudSplitPanel.setHorizontal", Horizontal);
    }
}
