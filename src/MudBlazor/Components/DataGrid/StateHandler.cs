// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.DataGrid
{
#nullable enable
    internal class StateHandler<T>
    {
        private T? _lastValue;
        private T? _value;
        private EventCallback<T> _event;

        internal T? Value
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

        internal StateHandler(T? state) 
        {
            _lastValue = _value = state;
            _event = default;
        }

        internal StateHandler(T? state, EventCallback<T> ev)
        {
            _lastValue = _value = state;
            _event = ev;
        }

        /// <summary>
        /// Call this in OnParametersSetAsync to update state where external parameters have changed
        /// </summary>
        /// <param name="parameterValue"></param>
        internal void SyncParameter(T parameterValue)
        {
            if (!EqualityComparer<T>.Default.Equals(_lastValue, parameterValue))
                _lastValue = _value = parameterValue;
        }
    }
#nullable disable
}
