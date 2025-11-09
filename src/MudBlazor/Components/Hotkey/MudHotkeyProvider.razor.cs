using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

#nullable enable

namespace MudBlazor;

public partial class MudHotkeyProvider : MudComponentBase
{
    private const string RegisterCallbackFunction = "mudHotkeyListener.registerCallbackFunction";
    private const string RegisterJsMethodName = "mudHotkeyListener.registerGlobalHotkey";

    [Parameter] public RenderFragment? ChildContent { get; set; }
    private Dictionary<string, Type> ComponentTypes { get; } = [];
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    private RenderFragment? _renderedFragment;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync(RegisterCallbackFunction, DotNetObjectReference.Create(this), "MudHotkeyProviderJsCallback");
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    internal async Task RegisterHotkeyAsync<T>(MudHotkey<T> hotkey)
    {
        ComponentTypes[hotkey.TypeofT.FullName!] = hotkey.TypeofT;
        await JsRuntime.InvokeVoidAsync(RegisterJsMethodName, hotkey.Key, hotkey.KeyModifiers, hotkey.TypeofT.FullName);
    }

    [JSInvokable]
    public void MudHotkeyProviderJsCallback(string componentName)
    {
        if (_renderedFragment is not null)
        {
            _renderedFragment = null;
            StateHasChanged();
        }
        
        var componentType = ComponentTypes[componentName];
        _renderedFragment = builder =>
        {
            #pragma warning disable IL2072
            builder.OpenComponent(0, componentType);
            #pragma warning restore IL2072
            builder.CloseComponent();
        };
        StateHasChanged();
    }
}
