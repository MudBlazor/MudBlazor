using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    /// <summary>
    /// see https://github.com/dotnet/aspnetcore/issues/13104
    /// </summary>
    [EventHandler("onmouseenter", typeof(MouseEventArgs), true, true)]
    [EventHandler("onmouseleave", typeof(MouseEventArgs), true, true)]
    public static class EventHandlers
    {

    }
}
