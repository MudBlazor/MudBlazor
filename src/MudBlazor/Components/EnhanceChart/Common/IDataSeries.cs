using System;

namespace MudBlazor.EnhanceChart
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
