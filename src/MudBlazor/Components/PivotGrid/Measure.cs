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
    public class Measure<T, TProperty> : BaseMeasure<T>
    {
#nullable enable
        private readonly Guid _id = Guid.NewGuid();

        private string? _aggregateName;
        private Func<IEnumerable<TProperty>, decimal?>? _cellContentFunc;

        private Func<IEnumerable<TProperty>, decimal>? _compiledAggregateFunc;


        private Expression<Func<IEnumerable<TProperty>, decimal>>? _lastAssignedAggregate;
        private Expression<Func<IEnumerable<TProperty>, decimal>>? _compiledAggregateFuncFor;

        [Parameter]
        [EditorRequired]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Expression<Func<IEnumerable<TProperty>, decimal>> Aggregate { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                              //= Expression.Lambda<Func<IEnumerable<TProperty>, decimal>>(Expression.Default(typeof(TProperty)), Expression.Parameter(typeof(T)));


        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedAggregate != Aggregate)
            {
                _lastAssignedAggregate = Aggregate;
                var compiledAggregateExpression = Aggregate.Compile();
                _cellContentFunc = item => compiledAggregateExpression(item);
            }

            var aggregate = PropertyPath.Visit(Aggregate);
            if (aggregate.IsBodyMemberExpression)
            {
                _aggregateName = aggregate.GetPath();
            }
            else
            {
                // Most likely this is a dynamic expression that people use as workaround https://try.mudblazor.com/snippet/cYGxuTmhyqAQeCVM
                // We can't assign any meaningful name at all, therefore we should assign an unique ID like we do for TemplateColumn
                _aggregateName = _id.ToString();
            }
            Title ??= aggregate.GetLastMemberName();
        }

        protected internal override object? AggregateFunc(IEnumerable<TProperty> item)
        {
            if (_compiledAggregateFunc == null || _compiledAggregateFuncFor != Aggregate)
            {
                _compiledAggregateFunc = Aggregate.Compile();
                _compiledAggregateFuncFor = Aggregate;
            }

            return _compiledAggregateFunc(item);
        }

        protected internal override Type AggregateType
            => typeof(T);

        protected internal override LambdaExpression? AggregateExpression
            => Aggregate;

        public override string? AggregateName => _aggregateName;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PivotGrid != null)
                PivotGrid.AddMeasure(this);
        }

    }
#nullable disable
}
