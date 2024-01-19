#nullable enable

using System;
using Microsoft.JSInterop;

namespace MudBlazor.Services;

public class RenderContext : IRenderContext
{

    private readonly IJSRuntime _jsRuntime;

    public RenderContext(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public bool IsInteractiveWebAssembly() => OperatingSystem.IsBrowser();

    public bool IsInteractiveServer() => !IsInteractiveWebAssembly() && IsInitialized();

    public bool IsStaticServer() => !IsInteractiveWebAssembly() && !IsInitialized();

    private bool IsInitialized()
    {
        return (bool)(_jsRuntime?.GetType()?.GetProperty("IsInitialized")?.GetValue(_jsRuntime) ?? false);
    }
}

