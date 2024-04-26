// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Expressions;

namespace MudBlazor
{
#nullable enable
    public abstract partial class BaseMeasure<T> : MudComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? Format { get; set; }

        [CascadingParameter]
        public MudPivotGrid<T>? PivotGrid { get; set; }

        protected internal virtual LambdaExpression? AggregateExpression { get; }

        public virtual string? AggregateName { get; }

        protected internal abstract object AggregateFunc(IEnumerable<TProperty> item);

        protected internal virtual Type? AggregateType { get; }
    }
#nullable disable
}
