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
using MudBlazor.Utilities;

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

        protected string _classname =>
            new CssBuilder("mud-table")
               .AddClass("mud-pivot-grid")               
               .AddClass("mud-table-dense", Dense)
               .AddClass("mud-table-hover", Hover)
               .AddClass("mud-table-bordered")
               .AddClass("mud-table-outlined", Outlined)
               .AddClass("mud-table-square", Square)
               .AddClass($"mud-elevation-{Elevation}", !Outlined)
              .AddClass(Class)
            .Build();

        /// <summary>
        /// Set true for rows with a narrow height
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// Set true to see rows hover on mouse-over.
        /// </summary>
        [Parameter] public bool Hover { get; set; }

        /// <summary>
        /// If true, table will be outlined.
        /// </summary>
        [Parameter] public bool Outlined { get; set; }

        /// <summary>
        /// Set true to disable rounded corners
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// Defines if the table has a horizontal scrollbar.
        /// </summary>
        [Parameter] public bool HorizontalScrollbar { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the table. If not set, it will try to grow in height. You can set this to any CSS value that the
        /// attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter] public string Height { get; set; }

        /// <summary>
        /// Add a class to the thead tag
        /// </summary>
        [Parameter] public string HeaderClass { get; set; }

        /// <summary>
        /// CSS class for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter] public string RowClass { get; set; }

        /// <summary>
        /// CSS styles for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter] public string RowStyle { get; set; }

        /// <summary>
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter] public Func<PivotTableColumnRender<T>, string> RowClassFunc { get; set; }

        /// <summary>
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter] public Func<PivotTableColumnRender<T>, string> RowStyleFunc { get; set; }


        protected string _style =>
            new StyleBuilder()
                .AddStyle("overflow-x", "auto", when: HorizontalScrollbar)
                .AddStyle(Style)
            .Build();

        protected string _tableStyle =>
            new StyleBuilder()
                .AddStyle("height", Height, !string.IsNullOrWhiteSpace(Height))
                .AddStyle("width", "max-content", when: HorizontalScrollbar)
                .AddStyle("display", "block", when: HorizontalScrollbar)
            .Build();
        protected string _tableClass =>
            new CssBuilder("mud-table-container")
            .Build();

        protected string _headClassname => new CssBuilder("mud-table-head")
            .AddClass(HeaderClass).Build();

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

        public PivotColumn<T> ColumnByName(string columnName)
        {
            return _pivot.ColumnByName(columnName);
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
