using System;

namespace MudBlazor.Utilities
{
    public interface ISnapshot<TSnapShot>
           where TSnapShot : IEquatable<TSnapShot>
    {
        protected TSnapShot OldSnapshotValue { get; set; }
        protected TSnapShot CreateSnapShot();

        public (Boolean,Boolean) SnapshotHasChanged(Boolean autoUpdate, Func<TSnapShot, TSnapShot,Boolean> additionalChecks)
        {
            var currentState = CreateSnapShot();
            if (currentState.Equals(OldSnapshotValue) == true)
            {
                return (false,false);
            }

            Boolean detailChanged = false;

            if (additionalChecks != null)
            {
                detailChanged = additionalChecks(OldSnapshotValue, currentState);
            }

            if (autoUpdate == true)
            {
                OldSnapshotValue = currentState;
            }

            return (true, detailChanged);
        }

        public Boolean SnapshotHasChanged(Boolean autoUpdate) => SnapshotHasChanged(autoUpdate, null).Item1;

        public void CreateSnapshot()
        {
            var currentState = CreateSnapShot();
            OldSnapshotValue = currentState;
        }
    }
}
