namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// Factory interface for creating instances of <see cref="IResizeObserver"/>.
/// </summary>
public interface IResizeObserverFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IResizeObserver"/>.
    /// </summary>
    /// <remarks>
    /// If you are creating this <see cref="IResizeObserver"/> instance yourself using this factory, then you need to manually call <see cref="ResizeObserver.DisposeAsync"/>; otherwise, you will get a memory leak.
    /// </remarks>
    /// <returns>A new instance of <see cref="IResizeObserver"/>.</returns>
    IResizeObserver Create();

    /// <summary>
    /// Creates a new instance of <see cref="IResizeObserver"/> with the specified options.
    /// </summary>
    /// <param name="options">The options to configure the resize observer.</param>
    /// <remarks>
    /// If you are creating this <see cref="IResizeObserver"/> instance yourself using this factory, then you need to manually call <see cref="ResizeObserver.DisposeAsync"/>; otherwise, you will get a memory leak.
    /// </remarks>
    /// <returns>A new instance of <see cref="IResizeObserver"/>.</returns>
    IResizeObserver Create(ResizeObserverOptions options);
}
