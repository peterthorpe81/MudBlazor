// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.DataGrid
{
#nullable enable
    public class ColumnState<T>
    {
        private StateHandler<bool> _hidden;

        internal ColumnState(StateHandler<bool> hidden) 
        {
            _hidden = hidden;
        }

        /// <summary>
        /// Call this in OnParametersSetAsync to update state where external parameters have changed
        /// </summary>
        /// <param name="hidden">Hidden parameter</param>
        internal void SyncParameters(bool hidden)
        {
            _hidden.SyncParameter(hidden);
        }

        public bool Hidden { get => _hidden.Value; set => _hidden.Value = value; }
    }
#nullable disable
}
