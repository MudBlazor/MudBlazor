
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

        Double LabelSize { get; }
        Double Margin { get;  }
        String LabelCssClass { get; }

        YAxisPlacement Placement { get; }

        Double MajorTickValue { get; }

        Guid Id { get; }
    }
}
