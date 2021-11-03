using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interop;

namespace MudBlazor.Services
{

    public delegate void KeyboardEvent(KeyboardEventArgs args);

    public interface IKeyInterceptor : IDisposable
    {
        Task Connect(string elementId, KeyInterceptorOptions options);
        Task Disconnect();

        event KeyboardEvent KeyDown;
        event KeyboardEvent KeyUp;

    }

}
