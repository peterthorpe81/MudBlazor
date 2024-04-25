// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Pivot
{
    public class ComplexPivotMeasure<T> : PivotMeasure<T>
    {
        public ComplexPivotMeasure(string propertyName, Func<decimal[], decimal> aggregate_function, params PivotMeasure<T>[] measures) : base(propertyName, (b) => 0)
        {
            values_aggregate = aggregate_function;
            this.measures = measures;
        }
        public PivotMeasure<T>[] measures { get; private set; }
        public Func<decimal[], decimal> values_aggregate { get; protected set; } = (values) => 0;
    }
}
