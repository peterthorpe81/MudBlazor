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
    public class Measure<T> : BaseMeasure<T>
    {
#nullable enable

        private string? _measureName;
       // private Func<T, decimal?>? _cellContentFunc;




        [Parameter]
        [EditorRequired] 
        public Expression<Func<IEnumerable<decimal>, decimal>> Aggregate { get; set; } = Expression.Lambda<Func<IEnumerable<decimal>, decimal>>(Expression.Default(typeof(decimal)), Expression.Parameter(typeof(IEnumerable<decimal>)));
        //Expression.Lambda<Func<T, TProperty>>(Expression.Default(typeof(TProperty)), Expression.Parameter(typeof(T)));

        [Parameter]
        [EditorRequired]
        public Expression<Func<T, decimal>> Value { get; set; } = Expression.Lambda<Func<T, decimal>>(Expression.Default(typeof(decimal)), Expression.Parameter(typeof(T)));


        private Expression<Func<T, decimal>>? _lastAssignedValue;
        private Expression<Func<IEnumerable<decimal>, decimal>>? _lastAssignedAggregate;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedValue != Value)
            {
                _lastAssignedValue = Value;
                Value.Compile();
                //var compiledValueExpression = Value.Compile();
                //_cellContentFunc = item => compiledValueExpression(item);
            }
            if (_lastAssignedAggregate != Aggregate)
            {
                _lastAssignedAggregate = Aggregate;
                Aggregate.Compile();
                //var compiledAggregateExpression = Aggregate.Compile();
                //_cellContentFunc = item => compiledAggregateExpression(item);
            }

            _measureName = $"{Value} {Aggregate}";
            var value = PropertyPath.Visit(Value);
            
            Title ??= value.GetLastMemberName();
        }


        private Func<T, decimal>? _compiledValueFunc;
        private Expression<Func<T, decimal>>? _compiledValueFuncFor;
        protected internal override decimal ValueFunc(T item)
        {
            if (_compiledValueFunc == null || _compiledValueFuncFor != Value)
            {
                _compiledValueFunc = Value.Compile();
                _compiledValueFuncFor = Value;
            }

            return _compiledValueFunc(item);
        }


        private Func<IEnumerable<decimal>, decimal>? _compiledAggregateFunc;
        private Expression<Func<IEnumerable<decimal>, decimal>>? _compiledAggregateFuncFor;
        protected internal override decimal AggregateFunc(IEnumerable<decimal> item)
        {
            if (_compiledAggregateFunc == null || _compiledAggregateFuncFor != Aggregate)
            {
                _compiledAggregateFunc = Aggregate.Compile();
                _compiledAggregateFuncFor = Aggregate;
            }

            return _compiledAggregateFunc(item);
        }


        protected internal override LambdaExpression? AggregateExpression
            => Aggregate;

        public override string? MeasureName => _measureName;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PivotGrid != null)
                PivotGrid.AddMeasure(this);
        }


    }
#nullable disable
}
