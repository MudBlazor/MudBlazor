// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interop;

namespace MudBlazor.Services
{

    public delegate void KeyboardEvent(KeyboardEventArgs args);

    public interface IKeyInterceptor
    {
        Task Connect(string elementId, KeyInterceptorOptions options);
        Task Disconnect();
        Task UpdateKey(KeyOptions option);
        void Dispose(); // Transient services can't be IDisposable but the service must still be (manually) deallocated

        event KeyboardEvent KeyDown;
        event KeyboardEvent KeyUp;
    }

}
