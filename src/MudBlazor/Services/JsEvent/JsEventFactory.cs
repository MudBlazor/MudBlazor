// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    public interface IJsEventFactory
    {
        IJsEvent Create();
    }

    public class JsEventFactory : IJsEventFactory
    {
        private readonly IServiceProvider _provider;

        public JsEventFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IJsEvent Create() =>
            new JsEvent(_provider.GetRequiredService<IJSRuntime>());
    }

}
