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
        .AddClass(Class)
        .Build();

    private string ClassnamePanel => new CssBuilder("child-panel")
        .AddClass("transparent", TransparentBackground)
        .AddClass(ClassPanel)
        .Build();

    private string ClassnameDivider => new CssBuilder("divider")
        .AddClass("horizontal", Horizontal)
        .AddClass(ClassDivider)
        .Build();
    
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? StylePanel { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? ClassPanel { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? StyleDivider { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? ClassDivider { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public bool Horizontal { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public bool TransparentBackground { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public RenderFragment? FirstPanel { get; set; }

    /// <summary>
    /// 
    /// </summary>
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
