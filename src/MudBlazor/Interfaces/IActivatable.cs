using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Interfaces
{
    public interface IActivatable
    {
        void Activate(object activator, MouseEventArgs args);
    }
}
