#nullable enable

namespace MudBlazor.Services;

/// <summary>
/// Service to return the current render mode
/// </summary>
public interface IRenderContext
{

    /// <summary>
    /// WebAssembly interctive mode
    /// </summary>
    /// <returns>true if the mode is active else false</returns>
    bool IsInteractiveWebAssembly();

    /// <summary>
    /// Server Side interctive mode
    /// </summary>
    /// <returns>true if the mode is active else false</returns>
    bool IsInteractiveServer();

    /// <summary>
    /// The server is statically rendering the page for SSR or PreRendering.
    /// No interactivity is available.
    /// </summary>
    /// <returns>true if the mode is active else false</returns>
    bool IsStaticServer();
}

