using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Handles activation of a component by a click-like event.
    /// </summary>
    public interface IActivatable
    {
        void Activate(object activator, MouseEventArgs args);
    }
}
