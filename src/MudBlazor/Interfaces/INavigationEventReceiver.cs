using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
#nullable enable
    public interface INavigationEventReceiver
    {
        Task OnNavigation();
    }
}
