using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
#pragma warning disable CS1998
#pragma warning disable CS0067
    public class MockJsEvent : IJsEvent
    {
        public void Dispose()
        {
            
        }

        public Task Connect(string element, JsEventOptions options)
        {
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            return Task.CompletedTask;
        }

        public event Action<int> CaretPositionChanged;
        public event Action<string> Paste;
        public event Action Copy;
        public event Action<int, int> Select;
    }
}
