using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Interfaces
{
#nullable enable
    public interface IActivatable
    {
        void Activate(object activator, MouseEventArgs args);
    }
}
