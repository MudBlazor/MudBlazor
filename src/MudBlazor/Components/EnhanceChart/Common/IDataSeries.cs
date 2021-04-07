using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    /// <summary>
    /// Representing a generell dataseries
    /// </summary>
    public interface IDataSeries
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
        /// The values of this series
        /// </summary>
        public IList<Double> Points { get; }

        /// <summary>
        /// The uniquq id of the series
        /// </summary>
        public Guid Id { get; }
    }
}
