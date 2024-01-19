#nullable enable

namespace MudBlazor.Services;

/// <summary>
/// Service to return the current render mode
/// </summary>
public interface IRenderContext
{

    /// <summary>
    /// Is the rendering in Interactive WebAssembly mode
    /// </summary>
    /// <returns>true if the mode is active else false</returns>
    bool IsInteractiveWebAssembly();

    /// <summary>
    /// Is the rendering in Interactive Server mode
    /// </summary>
    /// <returns>true if the mode is active else false</returns>
    bool IsInteractiveServer();

    /// <summary>
    /// Is the rendering in Static mode (SSR or PreRendering)
    /// Note: No interactivity is available.
    /// </summary>
    /// <returns>true if the mode is active else false</returns>
    bool IsStaticServer();
}

