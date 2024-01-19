#nullable enable

using System;
using Microsoft.JSInterop;

namespace MudBlazor.Services;

/// <summary>
/// This service returns the current render mode
/// Note it uses a private property of the IJSRuntime that is unsupported
/// At the time of writing MS don't provide a supported equivalent method
/// </summary>
public class RenderContext : IRenderContext
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Constructor
    /// IJSRuntime is injected for detection method IsInitialised
    /// </summary>
    public RenderContext(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <inhertidoc />
    public bool IsInteractiveWebAssembly() => OperatingSystem.IsBrowser();

    /// <inhertidoc />
    public bool IsInteractiveServer() => !IsInteractiveWebAssembly() && IsInitialized();

    /// <inhertidoc />
    public bool IsStaticServer() => !IsInteractiveWebAssembly() && !IsInitialized();

    private bool IsInitialized()
    {
        return (bool)(_jsRuntime?.GetType()?.GetProperty("IsInitialized")?.GetValue(_jsRuntime) ?? false);
    }
}

