using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Pivot;

namespace MudBlazor
{
    [RequiresUnreferencedCode("Calls System.Linq.Expressions.Expression.Property(Expression, String)")]
    public partial class MudPivotGrid<T> : MudComponentBase {
        private PivotTableRenderOption<T> Option { get; set; }
        private bool IsVertical => Options.MeasureArrangement == MeasureArrangementType.Vertical;

        [Parameter]
        public IEnumerable<T> Items { get; set; }

        [Parameter]
        public List<PivotColumn<T>> Columns { get; set; }

        [Parameter]
        public List<PivotColumn<T>> Rows { get; set; }

        [Parameter]
        public List<PivotMeasure<T>> Measures { get; set; }

        [Parameter]
        public PivotTableRenderOption<T> Options { get; set; }

        private PivotTable<T> _pivot;

        protected override async Task OnParametersSetAsync()
        {

            if (Items != null)
            {
                _pivot = new PivotTable<T>(Items, Rows, Columns, Measures);

                RowRender = new PivotTableHeaderRender<T>(HeaderType.Row, _pivot.ColHeaders, Options);
                ColRender = new PivotTableHeaderRender<T>(HeaderType.Column, _pivot.RowHeaders, Options);

            }
            await base.OnParametersSetAsync();
        }


        /// <summary>
        /// Header Row Rendering Count
        /// </summary>
        private int ColHeaderRowCount => RowRender.MaxDepth + (IsVertical ? 0 : 1);

        /// <summary>
        /// Header Col Rendering Count
        /// </summary>
        private int RowHeaderColCount => ColRender.MaxDepth + (IsVertical ? 1 : 0);

        private int HorisontalMeasureRatio => IsVertical ? 1 : _pivot.Measures.Count();
        private int VerticalMeasureRatio => IsVertical ? _pivot.Measures.Count() : 1;

        /// <summary>
        /// Header Row Rendering Engine
        /// </summary>
        private PivotTableHeaderRender<T> RowRender { get; set; }
        /// <summary>
        /// Header Col Rendering Engine
        /// </summary>
        private PivotTableHeaderRender<T> ColRender { get; set; }
        

    }
}
