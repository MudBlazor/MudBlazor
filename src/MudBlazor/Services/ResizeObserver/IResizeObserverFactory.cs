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
    /// <returns>A new instance of <see cref="IResizeObserver"/>.</returns>
    IResizeObserver Create();

    /// <summary>
    /// Creates a new instance of <see cref="IResizeObserver"/> with the specified options.
    /// </summary>
    /// <param name="options">The options to configure the resize observer.</param>
    /// <returns>A new instance of <see cref="IResizeObserver"/>.</returns>
    IResizeObserver Create(ResizeObserverOptions options);
}
