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
    public interface IKeyInterceptorFactory
    {
        public IKeyInterceptor Create();
    }

    public class KeyInterceptorFactory : IKeyInterceptorFactory
    {
        private readonly IServiceProvider _provider;

        public KeyInterceptorFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IKeyInterceptor Create() =>
            new KeyInterceptor(_provider.GetRequiredService<IJSRuntime>());
    }
}
