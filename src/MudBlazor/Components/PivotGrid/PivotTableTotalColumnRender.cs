using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Pivot;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class PivotTableTotalColumnRender<T> : PivotTableColumnRender<T> {
        public PivotTableTotalColumnRender(PivotHeaderCell<T> current, MudPivotGrid<T> grid, PivotAxisRenderOption headerOption, PivotTableColumnRender<T> parent = null) : base(current, grid, headerOption, parent) {
            Contract.Requires(headerOption != null);

            Title = headerOption.TotalTitle;
            //CssClass = new CssBuilder(grid._cellClass).AddClass(headerOption.TotalCssClass);            
            //CssClass += " " + headerOption.TotalCssClass;
        }
    }
}
