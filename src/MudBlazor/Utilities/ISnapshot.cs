using System;

namespace MudBlazor.Utilities
{
    public interface ISnapshot<TSnapShot>
           where TSnapShot : IEquatable<TSnapShot>
    {
        protected TSnapShot OldSnapshotValue { get; set; }
        protected TSnapShot CreateSnapShot();

        public Boolean SnapshotHasChanged(Boolean autoUpdate)
        {
            var currentState = CreateSnapShot();
            if (currentState.Equals(OldSnapshotValue) == true)
            {
                return false;
            }

            if (autoUpdate == true)
            {
                OldSnapshotValue = currentState;
            }

            return true;
        }

        public void CreateSnapshot()
        {
            var currentState = CreateSnapShot();
            OldSnapshotValue = currentState;
        }
    }
}
