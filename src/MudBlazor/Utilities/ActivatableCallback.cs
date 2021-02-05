using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    public class ActivatableCallback : IActivatable
    {
        public Action<object, MouseEventArgs> ActivateCallback { get; set; }

        public void Activate(object sender, MouseEventArgs args)
        {
            ActivateCallback?.Invoke(sender, args);
        }
    }
}
