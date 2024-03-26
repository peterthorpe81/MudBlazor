using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Pivot;

namespace MudBlazor 
{
    public partial class PivotTableRender<T> : MudComponentBase {
        private PivotTable<T> Table { get; set; }
        private PivotTableRenderOption<T> Option { get; set; }
        private bool IsVertical => Option.MeasureArrangement == MeasureArrangementType.Vertical;

        /// <summary>
        /// Header Row Rendering Count
        /// </summary>
        private int ColHeaderRowCount => RowRender.MaxDepth + (IsVertical ? 0 : 1);

        /// <summary>
        /// Header Col Rendering Count
        /// </summary>
        private int RowHeaderColCount => ColRender.MaxDepth + (IsVertical ? 1 : 0);

        private int HorisontalMeasureRatio => IsVertical ? 1 : Table.Measures.Count();
        private int VerticalMeasureRatio => IsVertical ? Table.Measures.Count() : 1;

        /// <summary>
        /// Header Row Rendering Engine
        /// </summary>
        private PivotTableHeaderRender<T> RowRender { get; set; }
        /// <summary>
        /// Header Col Rendering Engine
        /// </summary>
        private PivotTableHeaderRender<T> ColRender { get; set; }
        internal PivotTableRender(PivotTable<T> pivotTable, PivotTableRenderOption<T> option) {
            Table = pivotTable;
            Option = option;
            RowRender = new PivotTableHeaderRender<T>(HeaderType.Row, pivotTable.ColHeaders, option);
            ColRender = new PivotTableHeaderRender<T>(HeaderType.Column, pivotTable.RowHeaders, option);


        }
        /*public string Run() {
            return RenderTable(RenderHeaderRows() + RenderRows());
        }*/

        /*private string RenderMeasureTitleCell(PivotMeasure<T> measure) {
            return $"<th class=\"{Option.MeasureTitleCssClass}\">{ measure.PropertyName}</th>";
        }*/
        /*private string RenderRows() {
            
        }*/

    }
}
