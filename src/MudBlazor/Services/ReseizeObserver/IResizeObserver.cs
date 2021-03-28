// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{

    public delegate void SizeChanged(IDictionary<ElementReference,BoundingClientRect> changes);

    public interface IResizeObserver : IAsyncDisposable
    {
        Task<BoundingClientRect> Observe(ElementReference element);
        Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements);
        Task Unobserve(ElementReference element);

        Double GetWidth(ElementReference reference);
        Double GetHeight(ElementReference reference);
        BoundingClientRect GetSizeInfo(ElementReference reference);

        event SizeChanged OnResized;

        Boolean IsElementObserved(ElementReference reference);
    }
}
