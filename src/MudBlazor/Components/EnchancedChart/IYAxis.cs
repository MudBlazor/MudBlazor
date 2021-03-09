
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.EnchancedChart
{
    public interface IYAxis
    {
        void TickUpdated(Tick tick);
        Double Min { get; }
        Double Max { get; }
        Boolean ScalesAutomatically { get; }

        Guid Id { get; }
    }
}
