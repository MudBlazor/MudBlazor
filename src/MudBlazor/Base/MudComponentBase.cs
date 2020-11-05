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
        /// <summary>
        /// User class names, separated by space
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// User styles, applied on top of the component's own classes and styles
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Use Tag to attach any user data object to the component for your convenience.
        /// </summary>
        [Parameter] public object Tag { get; set; }

        /// <summary>
        /// UserAttributes carries all attributes you add to the component that don't match any of its parameters. They
        /// will be splatted onto the underlying HTML tag.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();
    }
}
