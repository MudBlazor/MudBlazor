using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
#nullable enable
    /// <summary>
    /// Interface for observing resize events on elements.
    /// </summary>
    public interface IResizeObserver : IAsyncDisposable
    {
        /// <summary>
        /// Event triggered when the size of an observed element changes.
        /// </summary>
        event SizeChanged OnResized;

        /// <summary>
        /// Observes the specified element for resize events.
        /// </summary>
        /// <param name="element">The element to observe.</param>
        /// <returns>A task that represents the asynchronous operation, containing the bounding client rectangle of the observed element.</returns>
        Task<BoundingClientRect?> Observe(ElementReference element);

        /// <summary>
        /// Observes the specified elements for resize events.
        /// </summary>
        /// <param name="elements">The elements to observe.</param>
        /// <returns>A task that represents the asynchronous operation, containing the bounding client rectangles of the observed elements.</returns>
        Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements);

        /// <summary>
        /// Stops observing the specified element for resize events.
        /// </summary>
        /// <param name="element">The element to stop observing.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Unobserve(ElementReference element);

        /// <summary>
        /// Gets the width of the specified element.
        /// </summary>
        /// <param name="reference">The element reference.</param>
        /// <returns>The width of the element.</returns>
        double GetWidth(ElementReference reference);

        /// <summary>
        /// Gets the height of the specified element.
        /// </summary>
        /// <param name="reference">The element reference.</param>
        /// <returns>The height of the element.</returns>
        double GetHeight(ElementReference reference);

        /// <summary>
        /// Gets the size information of the specified element.
        /// </summary>
        /// <param name="reference">The element reference.</param>
        /// <returns>The bounding client rectangle of the element.</returns>
        BoundingClientRect? GetSizeInfo(ElementReference reference);

        /// <summary>
        /// Checks if the specified element is being observed.
        /// </summary>
        /// <param name="reference">The element reference.</param>
        /// <returns>True if the element is being observed, otherwise false.</returns>
        bool IsElementObserved(ElementReference reference);
    }
}
