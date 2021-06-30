// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.EnhanceChart
{
    /// <summary>
    /// Representing a generell data element 
    /// </summary>
    public interface IChartDataElement
    {
        /// <summary>
        /// Toogle the enabled (visibility) state of the series
        /// </summary>
        void ToggleEnabledState();

        /// <summary>
        /// A value indicating if this series should be highlighted in the cart.
        /// </summary>
        Boolean IsActive { get; }

        /// <summary>
        /// If this value is false, the series is not rendered on the chart
        /// </summary>
        Boolean IsEnabled { get; }

        /// <summary>
        /// Mark the series as active
        /// </summary>
        void SetAsActive();

        /// <summary>
        /// Mark this series as inactive (not highlighted on the chart)
        /// </summary>
        void SetAsInactive();

        /// <summary>
        /// Sendign a request so that this series can become the only one that is active ont the chart
        /// </summary>
        void SentRequestToBecomeActiveAlone();

        /// <summary>
        /// Sending a reuqest to remove the exclusive active state, so that other series can become active too
        /// </summary>
        void RevokeExclusiveActiveState();

        /// <summary>
        /// The uniquq id of the series
        /// </summary>
        Guid Id { get; }
    }
}
