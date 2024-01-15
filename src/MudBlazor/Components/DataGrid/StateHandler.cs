// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.DataGrid
{
#nullable enable
    internal class StateHandler
    {
        internal static StateHandler<T?> Create<T>(T? state, EventCallback<T?> ev = default) where T : struct => new StateHandler<T?>(state, ev);
        internal static StateHandler<T> Create<T>(T state, EventCallback<T> ev = default) where T : struct => new StateHandler<T>(state, ev);        
        internal static StateHandler<string> Create<T>(string state, EventCallback<string> ev = default) => new StateHandler<string>(state, ev);
        internal static StateHandler<string?> Create(string? state, EventCallback<string?> ev = default) => new StateHandler<string?>(state, ev);
    }

    internal class StateHandler<T>
    {
        private T _lastValue;
        private T _value;
        private EventCallback<T> _event;

        internal T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    if (_event.HasDelegate)
                    {
                        _lastValue = value;
                        _event.InvokeAsync(value);
                    }
                }
            }
        }

        internal StateHandler(T state, EventCallback<T> ev = default)
        {
            _lastValue = _value = state;
            _event = ev;
        }

        /// <summary>
        /// Update state where external parameters have changed. Usually called in OnParametersSetAsync.
        /// </summary>
        /// <param name="parameterValue"></param>
        internal void SyncParameter(T parameterValue)
        {
            if (!EqualityComparer<T>.Default.Equals(_lastValue, parameterValue))
                _lastValue = _value = parameterValue;
        }
    }
}
