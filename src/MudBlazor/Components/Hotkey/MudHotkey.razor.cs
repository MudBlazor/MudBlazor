#nullable enable
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudHotkey<T> : MudComponentBase
{
    [Parameter] public JsKey Key { get; set; }
    [Parameter] public List<JsKeyModifier> KeyModifiers { get; set; } = [];
    [Parameter] public bool Rerender { get; set; } = true;
    [CascadingParameter] private MudHotkeyProvider? Provider { get; set; }
    
    public Type TypeofT => typeof(T);
    
    protected override void OnInitialized()
    {
        Provider?.RegisterHotkeyAsync(this);
    }
}

