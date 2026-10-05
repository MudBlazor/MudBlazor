using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Receives a notification when navigation occurs so a component can respond.
    /// </summary>
    public interface INavigationEventReceiver
    {
        Task OnNavigation();
    }
}
