// Not Used

using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor.Interop
{
    public class TouchEvent
    {
        public bool AltKey { get; set; }

        public Touch[] ChangedTouches { get; set; }

        public bool CtrlKey { get; set; }

        public bool MetaKey { get; set; }

        public bool ShiftKey { get; set; }

        public Touch[] TargetTouches { get; set; }

        public Touch[] Touches { get; set; }
    }
}
