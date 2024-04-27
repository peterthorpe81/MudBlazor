using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public enum Axis { Row, Column, None }
    public enum MeasureArrangementType { Horizontal, Vertical }
    public enum TotalPosition { Start, End, None }

    public class PivotAxisRenderOption
    {
        //public string TotalCssClass { get; set; } = "mud-pivot-total";
        public string TotalTitle { get; set; } = "Total";
        public TotalPosition TotalPosition { get; set; } = TotalPosition.End;
        public bool ShowTotalsForSingleValues { get; set; } = false;
    }

    [RequiresUnreferencedCode("Calls System.Linq.Expressions.Expression.Property(Expression, String)")]
    [CascadingTypeParameter(nameof(T))]
    public partial class MudPivotGrid<T> : MudComponentBase
    {
        //private PivotTableRenderOption<T> Option { get; set; }
        private bool IsVertical => MeasureArrangement == MeasureArrangementType.Vertical;

        [Parameter]
        public IEnumerable<T> Items { get; set; }




        [Parameter]
        public bool HighlightCrossedTotals { get; set; } = true;



        //[Parameter]
        //public PivotTableRenderOption<T> Options { get; set; }

        private PivotTable<T> _pivot;

        private List<PivotColumn<T>> _columns { get; set; }

        private List<PivotColumn<T>> _rows { get; set; }

        public List<PivotMeasure<T>> _measures { get; set; }


        /// <summary>
        /// Set true for rows with a narrow height
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Set true to see rows hover on mouse-over.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Hover { get; set; }

        /// <summary>
        /// If true, table will be outlined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Outlined { get; set; }

        /// <summary>
        /// Set true to disable rounded corners
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Defines if the table has a horizontal scrollbar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool HorizontalScrollbar { get; set; }


        /// <summary>
        /// Defines if the table has fixed headers
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool FixedHeader { get; set; } = false;

        /// <summary>
        /// Setting a height will allow to scroll the table. If not set, it will try to grow in height. You can set this to any CSS value that the
        /// attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public string Height { get; set; }

        /// <summary>
        /// The color of the component, used to highlight totals. A CSS fitler is applied for total depth.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Add a class to the thead tag
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public string HeaderClass { get; set; }

        /// <summary>
        /// CSS class for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public string RowClass { get; set; }

        /// <summary>
        /// Measures are arranged Horizontally or vertically
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)] 
        public MeasureArrangementType MeasureArrangement { get; set; } = MeasureArrangementType.Horizontal;

        /// <summary>
        /// Row totals display above, below or none
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public TotalPosition RowTotalPosition { get; set; } = TotalPosition.End;

        /// <summary>
        /// Column totals display above, below or none
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public TotalPosition ColTotalPosition { get; set; } = TotalPosition.End;

        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool ShowRowTotalsForSingleValues { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool ShowColTotalsForSingleValues { get; set; } = false;

        /// <summary>
        /// Header titles are shown
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool ShowHeaderTitles { get; set; } = true;




        /// <summary>
        /// The Fields that make up the pivot grid. Add Field components to this RenderFragment.
        /// </summary>
        [Parameter] public RenderFragment Fields { get; set; }

        /// <summary>
        /// The Measures that make up the pivot grid. Add Measure components to this RenderFragment.
        /// </summary>
        [Parameter] public RenderFragment Measures { get; set; }

        public readonly List<BaseField<T>> FieldSet = new List<BaseField<T>>();
        public readonly List<BaseMeasure<T>> MeasureSet = new List<BaseMeasure<T>>();

        internal void AddField(BaseField<T> field)
        {
            FieldSet.Add(field);
        }

        internal void AddMeasure(BaseMeasure<T> measure)
        {
            MeasureSet.Add(measure);
        }

        protected string _classname =>
           new CssBuilder("mud-table")
              .AddClass("mud-pivot-grid")
              .AddClass("mud-table-dense", Dense)
              .AddClass("mud-table-hover", Hover)
              .AddClass("mud-table-outlined", Outlined)
              .AddClass("mud-table-square", Square)
              .AddClass($"mud-elevation-{Elevation}", !Outlined)

             //.AddClass(Class)
           .Build();

        protected string _style =>
            new StyleBuilder()
                .AddStyle("overflow-x", "auto", when: HorizontalScrollbar)
            .AddStyle("--pivot-color", $"var(--mud-palette-{Color.ToDescriptionString()})")
            .AddStyle("--pivot-color-text", $"var(--mud-palette-{Color.ToDescriptionString()}-text)")
             //   .AddStyle(Style)
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
            .AddClass("mud-pivot-grid-sticky-header", FixedHeader)
            .AddClass(HeaderClass).Build();

        protected string _headRowClassname => new CssBuilder("mud-pivot-head-row").Build();
        protected string _rowClass => new CssBuilder("mud-table-row").AddClass(RowClass).Build();
        protected string _cornerClass => new CssBuilder("corner").AddClass("mud-table-cell").Build();


        internal string MeasureClass(CellType colType, int colDepth, CellType rowType, int rowDepth/*, int cssDepth*/)
        {
            int cssDepth = Math.Max(rowDepth, colDepth);

            if (colType == CellType.Value && rowType == CellType.Value)
                cssDepth--;

            if (HighlightCrossedTotals && rowType != CellType.Value && colType != CellType.Value)
            {
                cssDepth++;
            }
            /*if (colType == CellType.SubTotal && rowType != CellType.Value)
                cssDepth++;

            if (colType == CellType.GrandTotal && rowType != CellType.Value)
                cssDepth = cssDepth + 2;*/


            return new CssBuilder("mud-table-cell")
                .AddClass($"measure")
                .AddClass($"td-{cssDepth}")
                .AddClass($"cd-{colDepth}")
                .AddClass($"rd-{rowDepth}")
                .AddClass($"rt-{colType}")
                .AddClass($"ct-{rowType}").Build();
        }

        internal string MeasureTitleClass(CellType type, int cssDepth)
        {
            if (type == CellType.Value)
                cssDepth--;

            return new CssBuilder("mud-table-cell")
                .AddClass("measure-title")
                .AddClass($"td-{cssDepth}")
                .AddClass($"mt-{type}")
                .Build();
        }

        internal string RowTitleClass(int cssDepth)
        {
            return new CssBuilder("mud-table-cell")
                .AddClass($"row-title")
                .AddClass($"td-{cssDepth}").Build();
        }
        
        internal string ColTitleClass(int cssDepth)
        {
            return new CssBuilder("mud-table-cell")
                .AddClass($"col-title")
                .AddClass($"td-{cssDepth}").Build();
        }

        internal string RowHeaderClass(CellType rowType, int rowDepth)
        {
            return new CssBuilder("mud-table-cell")
                .AddClass($"r-header")
                .AddClass($"td-{rowDepth}")
                .AddClass($"rd-{rowDepth}")
                .AddClass($"rt-{rowType}").Build();
        }

        internal string ColHeaderClass(CellType colType, int colDepth)
        {
            return new CssBuilder("mud-table-cell")
                .AddClass($"c-header")
                .AddClass($"td-{colDepth}")
                .AddClass($"cd-{colDepth}")
                .AddClass($"ct-{colType}").Build();
        }


        internal Dictionary<Axis, PivotAxisRenderOption> Header;
        //bool _isFirstRendered = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_parametersUpdated)
            {
                _parametersUpdated = false;
                BuildPivot();
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        bool _parametersUpdated;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            _parametersUpdated = true;
        }

        private void BuildPivot()
        {
            Header = new Dictionary<Axis, PivotAxisRenderOption>() {
                { Axis.Row , new PivotAxisRenderOption() { TotalPosition = RowTotalPosition, TotalTitle =  Localizer["MudPivotGrid.GrandTotal"], ShowTotalsForSingleValues = ShowRowTotalsForSingleValues  } },
                { Axis.Column , new PivotAxisRenderOption() { TotalPosition = ColTotalPosition, TotalTitle = Localizer["MudPivotGrid.GrandTotal"], ShowTotalsForSingleValues = ShowColTotalsForSingleValues   } }
            };

            _rows = new List<PivotColumn<T>>();
            _columns = new List<PivotColumn<T>>();
            foreach (var field in FieldSet)
            {
                if (field.Axis == Axis.Row)
                    _rows.Add(new PivotColumn<T>(field.PropertyName, options: new PivotAxisRenderOption()
                    {
                        ShowTotalsForSingleValues = field.ShowTotalsForSingleValues ?? ShowRowTotalsForSingleValues,
                        TotalTitle = field.TotalTitle ?? Localizer["MudPivotGrid.Total"],
                        TotalPosition = field.TotalPosition ?? RowTotalPosition
                    }));

                if (field.Axis == Axis.Column)
                    _columns.Add(new PivotColumn<T>(field.PropertyName, options: new PivotAxisRenderOption()
                    {
                        ShowTotalsForSingleValues = field.ShowTotalsForSingleValues ?? ShowColTotalsForSingleValues,
                        TotalTitle = field.TotalTitle ?? Localizer["MudPivotGrid.Total"],
                        TotalPosition = field.TotalPosition ?? ColTotalPosition
                    }));
            }

            _measures = new List<PivotMeasure<T>>();
            foreach (var measure in MeasureSet)
            {
                var m = new PivotMeasure<T>(measure.MeasureName, title: measure.Title);
                m.ValueGetter = measure.ValueFunc;
                if (measure is DivisorMeasure<T>)
                    m.aggregate = measure.ComplexAggregateFunc;
                else
                    m.aggregate = measure.AggregateFunc; 
                m.Format = measure.Format;
               
                _measures.Add(m);
            }

            if (Items != null)
            {
                _pivot = new PivotTable<T>(Items, _rows, _columns, _measures);
                RowRender = new PivotTableHeaderRender<T>(Axis.Row, _pivot.ColHeaders, this);
                ColRender = new PivotTableHeaderRender<T>(Axis.Column, _pivot.RowHeaders, this);
            }
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
