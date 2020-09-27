using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudComponentBase : ComponentBase
    {

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Use this to attach any user data object to the component for your convenience.
        /// </summary>
        [Parameter] public object Tag { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> UserAttributes { get; set; }
    }
}
