// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.EnchancedChart
{
    public interface IDataSeries
    {
        void ToggleEnabledState();

        Boolean IsActive { get; }
        void SetAsActive();
        void SetAsInactive();

        void SentRequestToBecomeActiveAlone();
        void RevokeExclusiveActiveState();
    }
}
