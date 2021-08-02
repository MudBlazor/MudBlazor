// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockEventListener : IEventListener
    {
        public Dictionary<Guid, Func<object, Task>> Callbacks { get; private set; } = new();

        public Dictionary<Guid, string> ElementIdMapper { get; private set; } = new();

        public Task<Guid> Subscribe<T>(string eventName, string elementId, string projection, int throttleInterval, Func<object, Task> callback)
        {
            var id = Guid.NewGuid();
            ElementIdMapper.Add(id, elementId);
            Callbacks.Add(id, callback);
            return Task.FromResult(id);
        }

        public Task<bool> Unsubscribe(Guid key)
        {
            var result = Callbacks.ContainsKey(key);
            if (result == true)
            {
                Callbacks.Remove(key);
                ElementIdMapper.Remove(key);
            }

            return Task.FromResult(result);
        }

        internal void FireEvent(MouseEventArgs args)
        {
            foreach (var item in Callbacks.Values)
            {
                item.Invoke(args);
            }
        }
    }
}
