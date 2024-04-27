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
using MudBlazor.Pivot;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Expressions;

namespace MudBlazor
{
    public class DivisorMeasure<T> : BaseMeasure<T>
    {
#nullable enable

        private string? _measureName;
        // private Func<T, decimal?>? _cellContentFunc;

        [Parameter]
        [EditorRequired]
        public Expression<Func<IEnumerable<decimal>, decimal>> NumeratorAggregate { get; set; } = Expression.Lambda<Func<IEnumerable<decimal>, decimal>>(Expression.Default(typeof(decimal)), Expression.Parameter(typeof(IEnumerable<decimal>)));
        //Expression.Lambda<Func<T, TProperty>>(Expression.Default(typeof(TProperty)), Expression.Parameter(typeof(T)));

        [Parameter]
        [EditorRequired]
        public Expression<Func<T, decimal>> NumeratorValue { get; set; } = Expression.Lambda<Func<T, decimal>>(Expression.Default(typeof(decimal)), Expression.Parameter(typeof(T)));

        [Parameter]
        [EditorRequired]
        public Expression<Func<IEnumerable<decimal>, decimal>> DenominatorAggregate { get; set; } = Expression.Lambda<Func<IEnumerable<decimal>, decimal>>(Expression.Default(typeof(decimal)), Expression.Parameter(typeof(IEnumerable<decimal>)));
        //Expression.Lambda<Func<T, TProperty>>(Expression.Default(typeof(TProperty)), Expression.Parameter(typeof(T)));

        [Parameter]
        [EditorRequired]
        public Expression<Func<T, decimal>> DenominatorValue { get; set; } = Expression.Lambda<Func<T, decimal>>(Expression.Default(typeof(decimal)), Expression.Parameter(typeof(T)));


        private Expression<Func<T, decimal>>? _lastAssignedNumeratorValue;
        private Expression<Func<IEnumerable<decimal>, decimal>>? _lastAssignedNumeratorAggregate;
        private Expression<Func<T, decimal>>? _lastAssignedDenominatorValue;
        private Expression<Func<IEnumerable<decimal>, decimal>>? _lastAssignedDenominatorAggregate;


        private PivotMeasure<T>? numeratorMeasure { get; set; }
        private PivotMeasure<T>? denominatorMeasure { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            numeratorMeasure = new PivotMeasure<T>("Numerator");
            denominatorMeasure = new PivotMeasure<T>("Denominator");
            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedNumeratorValue != NumeratorValue)
            {
                _lastAssignedNumeratorValue = NumeratorValue;
                numeratorMeasure.ValueGetter = NumeratorValue.Compile();
            }
            if (_lastAssignedNumeratorAggregate != NumeratorAggregate)
            {
                _lastAssignedNumeratorAggregate = NumeratorAggregate;
                numeratorMeasure.aggregate = NumeratorAggregate.Compile();
            }
            if (_lastAssignedDenominatorValue != DenominatorValue)
            {
                _lastAssignedDenominatorValue = DenominatorValue;
                denominatorMeasure.ValueGetter = DenominatorValue.Compile();
            }
            if (_lastAssignedDenominatorAggregate != DenominatorAggregate)
            {
                _lastAssignedDenominatorAggregate = DenominatorAggregate;
                denominatorMeasure.aggregate = DenominatorAggregate.Compile();
            }

            _measureName = $"{NumeratorValue} {NumeratorAggregate} {DenominatorValue} {DenominatorAggregate}";
            var nValue = PropertyPath.Visit(NumeratorValue);
            var dValue = PropertyPath.Visit(DenominatorValue);
            Title ??= $"{nValue.GetLastMemberName()}/{dValue.GetLastMemberName()}";
        }

        protected internal override decimal ValueFunc(T item)
        {
            decimal? n = numeratorMeasure?.ValueGetter.Invoke(item);
            decimal? d = denominatorMeasure?.ValueGetter.Invoke(item);

            if (n is null || d is null || d == 0)
                return 0;

            return n.Value / d.Value;

            // denominatorMeasure.ValueGetter

            /*var values = new List<decimal>();
            foreach (var ms in measure.measures)
            {
                values.Add(GetValue(row, col, ms));
            }
            return measure.values_aggregate(values.ToArray());*/
        }

        protected internal override decimal AggregateFunc(IEnumerable<decimal> item)
        {
            var x = item.ToArray();
            return (x.Length == 2 && x[0] != 0) ? x[1] / x[0] : 0;
           // return 0;
        }

        protected internal override decimal ComplexAggregateFunc(IEnumerable<decimal> item)
        {

            var x = item.ToArray();
            return (x.Length == 2 && x[0] != 0) ? x[1] / x[0] : 0;
            //return (item.First() != 0) ? item.Skip(1).First() / item.First() : 0;
            //return (item.Length == 2 && item[0] != 0) ? (item[1] / item[0]) : 0;
        }

        protected internal override LambdaExpression? AggregateExpression
            => NumeratorAggregate;

        public override string? MeasureName => _measureName;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PivotGrid != null)
                PivotGrid.AddMeasure(this);
        }

#nullable disable
    }
}
