﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
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
