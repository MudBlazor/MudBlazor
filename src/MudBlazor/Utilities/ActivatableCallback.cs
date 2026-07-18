using System;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    /// <summary>
    /// Adapts an <see cref="IActivatable"/> activation into a delegate callback, letting types like <see cref="MudMessageBox"/> react to a button activation without implementing the interface themselves.
    /// </summary>
    public class ActivatableCallback : IActivatable
    {
        public Action<object, MouseEventArgs>? ActivateCallback { get; set; }

        public void Activate(object sender, MouseEventArgs args)
        {
            ActivateCallback?.Invoke(sender, args);
        }
    }
}
