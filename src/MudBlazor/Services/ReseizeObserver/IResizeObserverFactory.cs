// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Services
{
    public interface IResizeObserverFactory
    {
        IResizeObserver Create(ResizeObserverOptions options);
    }
}
