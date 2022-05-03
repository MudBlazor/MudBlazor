using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{

    public delegate void SizeChanged(IDictionary<ElementReference, BoundingClientRect> changes);

    public interface IResizeObserver
    {
        Task<BoundingClientRect> Observe(ElementReference element);
        Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements);
        Task Unobserve(ElementReference element);

        double GetWidth(ElementReference reference);
        double GetHeight(ElementReference reference);
        BoundingClientRect GetSizeInfo(ElementReference reference);

        event SizeChanged OnResized;

        bool IsElementObserved(ElementReference reference);

        void Dispose(); // Transient services can't be IDisposable but the service must still be (manually) deallocated
        ValueTask DisposeAsync(); // Transient services can't be IAsyncDisposable but the service must still be (manually) deallocated
    }
}
