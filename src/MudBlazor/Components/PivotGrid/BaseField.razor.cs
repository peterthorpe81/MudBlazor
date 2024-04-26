﻿// Copyright (c) MudBlazor 2021
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
    public abstract partial class BaseField<T> : MudComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        [EditorRequired]
        public Axis Axis { get; set; }

        [Parameter]
        public string? Format { get; set; }

        [CascadingParameter]
        public MudPivotGrid<T>? PivotGrid { get; set; }

        [Parameter]
        public TotalPosition? TotalPosition { get; set; } = MudBlazor.TotalPosition.End;

        [Parameter]
        public bool? ShowTotalsForSingleValues { get; set; } = false;

        protected internal virtual LambdaExpression? PropertyExpression { get; }

        public virtual string? PropertyName { get; }

        protected internal abstract object? PropertyFunc(T item);

        protected internal virtual Type? PropertyType { get; }

        protected internal string? TotalTitle { get; set; }
    }

    

#nullable disable
}
