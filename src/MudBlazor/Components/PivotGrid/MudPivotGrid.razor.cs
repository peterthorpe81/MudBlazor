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
    public enum HeaderType { Row, Column }
    public enum MeasureArrangementType { Horizontal, Vertical }
    public enum OutputPosition { Above, Below, None }

    public class PivotAxisRenderOption
    {
        public string TotalCssClass { get; set; } = "mud-pivot-total";
        public string TotalTitle { get; set; } = "Total";
        public OutputPosition TotalPosition { get; set; } = OutputPosition.Below;
        public bool ShowTotalsForSingleValues { get; set; } = false;
    }

    [RequiresUnreferencedCode("Calls System.Linq.Expressions.Expression.Property(Expression, String)")]
    public partial class MudPivotGrid<T> : MudComponentBase {
        //private PivotTableRenderOption<T> Option { get; set; }
        private bool IsVertical => MeasureArrangement == MeasureArrangementType.Vertical;

        [Parameter]
        public IEnumerable<T> Items { get; set; }

        [Parameter]
        public List<PivotColumn<T>> Columns { get; set; }

        [Parameter]
        public List<PivotColumn<T>> Rows { get; set; }

        [Parameter]
        public List<PivotMeasure<T>> Measures { get; set; }

        //[Parameter]
        //public PivotTableRenderOption<T> Options { get; set; }

        private PivotTable<T> _pivot;



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
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter] public Func<PivotTableColumnRender<T>, string> MeasureClassFunc { get; set; }

        /// <summary>
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter] public Func<PivotTableColumnRender<T>, string> MeasureStyleFunc { get; set; }

        /// <summary>
        /// Measures are arranged Horizontally or vertically
        /// </summary>
        [Parameter] public MeasureArrangementType MeasureArrangement { get; set; } = MeasureArrangementType.Horizontal;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        //public string TotalCssClass { get; set; } = "GrandTotal";
        //[Parameter] public string TotalTitle { get; set; } = Localizer["MudPivotGrid.GrandTotal"];
        //[Parameter] public bool RenderRowTotals { get; set; } = true;
        //[Parameter] public bool RenderColumnTotals { get; set; } = true;

        internal string TotalTitle { get; set; }

        [Parameter] public bool RenderHeaderTitles { get; set; } = true;
        [Parameter] public OutputPosition RowTotalPosition { get; set; } = OutputPosition.Below;
        [Parameter] public OutputPosition ColumnTotalPosition { get; set; } = OutputPosition.Below;

        protected string _classname =>
           new CssBuilder("mud-table")
              .AddClass("mud-pivot-grid")
              .AddClass("mud-table-dense", Dense)
              .AddClass("mud-table-hover", Hover)
              .AddClass("mud-table-outlined", Outlined)
              .AddClass("mud-table-square", Square)
              .AddClass($"mud-elevation-{Elevation}", !Outlined)

             .AddClass(Class)
           .Build();

        protected string _style =>
            new StyleBuilder()
                .AddStyle("overflow-x", "auto", when: HorizontalScrollbar)
            .AddStyle("--pivot-color", $"var(--mud-palette-{Color.ToDescriptionString()})")
            .AddStyle("--pivot-color-text", $"var(--mud-palette-{Color.ToDescriptionString()}-text)")
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

        protected string _totalClass =>
            new CssBuilder("mud-pivot-total").AddClass("mud-table-cell")
            .Build();

        protected string _headClassname => new CssBuilder("mud-table-head")
            .AddClass(HeaderClass).Build();

        protected string _headRowClassname => new CssBuilder("mud-pivot-head-row").Build();

        protected string _rowClass => new CssBuilder("mud-table-row").AddClass(RowClass).Build();

        protected string _measureTitleClass => new CssBuilder("mud-pivot-measure-title").AddClass("mud-table-cell").Build();
        protected string _rowTitleClass => new CssBuilder("mud-pivot-row-title").AddClass("mud-table-cell").Build();
        //protected string _columnTitleClass => new CssBuilder("mud-pivot-column-title").AddClass("mud-table-cell").Build();
        protected string _cornerClass => new CssBuilder("mud-pivot-corner").AddClass("mud-table-cell").Build();

        internal string CellClass => new CssBuilder("mud-table-cell").Build();
        internal string TotalClass => new CssBuilder("mud-pivot-total").Build();


         internal Dictionary<HeaderType, PivotAxisRenderOption> Header;

        protected override async Task OnParametersSetAsync()
        {

            TotalTitle = Localizer["MudPivotGrid.GrandTotal"];

            Header = new Dictionary<HeaderType, PivotAxisRenderOption>() {
                { HeaderType.Row , new PivotAxisRenderOption() { TotalPosition = RowTotalPosition, TotalTitle =  TotalTitle, TotalCssClass = TotalClass } },
                { HeaderType.Column , new PivotAxisRenderOption() { TotalPosition = ColumnTotalPosition, TotalTitle = TotalTitle, TotalCssClass = TotalClass  } }        
            };


            if (Items != null)
            {
                _pivot = new PivotTable<T>(Items, Rows, Columns, Measures);

                RowRender = new PivotTableHeaderRender<T>(HeaderType.Row, _pivot.ColHeaders, this);
                ColRender = new PivotTableHeaderRender<T>(HeaderType.Column, _pivot.RowHeaders, this);

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
