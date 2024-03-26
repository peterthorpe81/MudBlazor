using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Pivot;

namespace MudBlazor
{
    public class PivotTableTotalColumnRender<T> : PivotTableColumnRender<T> {
        public PivotTableTotalColumnRender(PivotHeaderCell<T> current, PivotTableRenderOption<T> option, PivotAxisRenderOption headerOption, PivotTableColumnRender<T> parent = null) : base(current, option, headerOption, parent) {
            Contract.Requires(headerOption != null);

            Title = headerOption.TotalTitle;
            CssClass += " " + headerOption.TotalCssClass;
        }
    }
}
