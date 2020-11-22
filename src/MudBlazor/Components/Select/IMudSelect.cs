using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Select
{
    internal interface IMudSelect
    {
        void CheckGenericTypeMatch(object select_item);
        bool MultiSelection { get; set; }

    }
}
