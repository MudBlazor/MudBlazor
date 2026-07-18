using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Receives a notification when navigation occurs, letting a component such as <see cref="MudDrawer"/> respond, for example by closing on a link click.
    /// </summary>
    public interface INavigationEventReceiver
    {
        Task OnNavigation();
    }
}
