#nullable enable
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudHotkey : MudComponentBase
{
    private const string RegisterJsMethodName = "mudHotkeyListener.registerHotkey";

    [Parameter, Category(CategoryTypes.Hotkey.Appearance)] public RenderFragment? ChildContent { get; set; }
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public JsKey Key { get; set; }
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public IEnumerable<JsKeyModifier> KeyModifiers { get; set; } = [];
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public EventCallback OnHotkeyPressed { get; set; }
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public bool HideChildContentOnRepress { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    private bool _childContentIsVisible;
    private bool _isRendered;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RegisterHotkeyAsync();
            _isRendered = true;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_isRendered) await RegisterHotkeyAsync();
    }

    private async Task RegisterHotkeyAsync()
    {
        await JsRuntime.InvokeVoidAsync(RegisterJsMethodName, DotNetObjectReference.Create(this), nameof(MudHotkeyProviderJsCallback), Key, KeyModifiers);
    }

    [JSInvokable]
    public async Task MudHotkeyProviderJsCallback()
    {
        if (!_childContentIsVisible)
        {
            _childContentIsVisible = true;
            StateHasChanged();
        }
        else if (HideChildContentOnRepress)
        {
            _childContentIsVisible = false;
            StateHasChanged();
        }

        await OnHotkeyPressed.InvokeAsync();
    }
}
