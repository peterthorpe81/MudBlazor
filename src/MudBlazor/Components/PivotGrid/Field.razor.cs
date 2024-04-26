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
    public class Field<T, TProperty> : MudComponentBase
    {

        [CascadingParameter] 
        public MudPivotGrid<T> PivotGrid { get; set; }


        [Parameter]
        public string TotalTitle { get; set; } = "Total";

        [Parameter]
        public OutputPosition TotalPosition { get; set; } = OutputPosition.Below;

        [Parameter]
        public bool ShowTotalsForSingleValues { get; set; } = false;

        private string? _propertyName;
        private Func<T, object?>? _cellContentFunc;
        private Func<T, TProperty>? _compiledPropertyFunc;
        private Expression<Func<T, TProperty>>? _lastAssignedProperty;
        private Expression<Func<T, TProperty>>? _compiledPropertyFuncFor;

        [Parameter]
        [EditorRequired]
        public Expression<Func<T, TProperty>> Property { get; set; } = Expression.Lambda<Func<T, TProperty>>(Expression.Default(typeof(TProperty)), Expression.Parameter(typeof(T)));

        [Parameter]
        public string? Format { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedProperty != Property)
            {
                _lastAssignedProperty = Property;
                var compiledPropertyExpression = Property.Compile();
                _cellContentFunc = item => compiledPropertyExpression(item);
            }

            var property = PropertyPath.Visit(Property);
            if (property.IsBodyMemberExpression)
            {
                _propertyName = property.GetPath();
            }
            else
            {
                // Most likely this is a dynamic expression that people use as workaround https://try.mudblazor.com/snippet/cYGxuTmhyqAQeCVM
                // We can't assign any meaningful name at all, therefore we should assign an unique ID like we do for TemplateColumn
                _propertyName = _id.ToString();
            }
            //Title ??= property.GetLastMemberName();
        }

        protected internal LambdaExpression? PropertyExpression  => Property;

        public string? PropertyName => _propertyName;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PivotGrid != null)
                PivotGrid.AddField(this);
        }
}
