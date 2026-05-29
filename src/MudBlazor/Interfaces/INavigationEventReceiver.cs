using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    public interface INavigationEventReceiver
    {
        Task OnNavigation();
    }
}
