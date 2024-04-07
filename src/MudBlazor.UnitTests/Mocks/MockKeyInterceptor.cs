using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
#pragma warning disable CS1998
#pragma warning disable CS0067
    public class MockKeyInterceptorServiceFactory : IKeyInterceptorFactory
    {
        private readonly MockKeyInterceptorService _interceptorService;

        public MockKeyInterceptorServiceFactory(MockKeyInterceptorService interceptorService)
        {
            _interceptorService = interceptorService;
        }

        public MockKeyInterceptorServiceFactory()
        {

        }

        public IKeyInterceptor Create() => _interceptorService ?? new MockKeyInterceptorService();
    }

    public class MockKeyInterceptorService : IKeyInterceptor
    {
        public void Dispose()
        {

        }

        public Task Connect(string element, KeyInterceptorOptions options)
        {
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            return Task.CompletedTask;
        }

        public Task UpdateKey(KeyOptions option)
        {
            return Task.CompletedTask;
        }

        public event KeyboardEvent KeyDown;
        public event KeyboardEvent KeyUp;
    }
}
