﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Pivot;

namespace MudBlazor
{
    public abstract class ComplexMeasure<T> : BaseMeasure<T>
    {
        protected internal abstract decimal ComplexAggregateFunc(IEnumerable<decimal> item);

        public PivotMeasure<T>[] measures { get; protected set; }

    }
}
