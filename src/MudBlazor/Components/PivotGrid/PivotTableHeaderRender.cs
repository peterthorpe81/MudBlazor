using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Pivot;

namespace MudBlazor
{
    public class PivotTableHeaderRender<T> {
        private List<PivotTableColumnRender<T>> rootCells = new List<PivotTableColumnRender<T>>();
        private List<PivotColumn<T>> Headers { get; set; }
        public int MaxDepth => Headers.Count;
        public string HeaderTitle(int depth) => Headers[depth].PropertyName;

        public PivotTableHeaderRender(Axis headerType, IEnumerable<PivotColumn<T>> headers, MudPivotGrid<T> grid) {
            Contract.Requires(headers != null);
            Contract.Requires(grid != null);

            //OutputPosition totalPosition = headerType == HeaderType.Column ? grid.ColumnTotalPosition : grid.RowTotalPosition;
            Headers = headers.ToList();
            var headerOption = grid.Header[headerType];
            if (headerOption.TotalPosition == TotalPosition.Start) {
                rootCells.Add(new PivotTableTotalColumnRender<T>(null, grid, headerOption));
            }

            var topLevelHeader = headers.FirstOrDefault();
            if (topLevelHeader is not null)
            {
                foreach (var cell in topLevelHeader.Items)
                {
                    rootCells.Add(new PivotTableColumnRender<T>(cell, grid, headerOption));
                }
            }
            else
            {
                rootCells.Add(new PivotTableTotalColumnRender<T>(null, grid, headerOption));
            }
            if (headerOption.TotalPosition == TotalPosition.End) {
                rootCells.Add(new PivotTableTotalColumnRender<T>(null, grid, headerOption));
            }
        }
        public IEnumerable<PivotTableColumnRender<T>> this[int depth] {
            get {
                foreach (var cell in rootCells) {
                    foreach (var c in cell.ListByDepth(depth)) {
                        yield return c;
                    }
                }
            }
        }
        public IEnumerable<PivotTableColumnRender<T>> Leaves {
            get {
                foreach (var cell in rootCells) {
                    foreach (var c in cell.Leaf) {
                        yield return c;
                    }
                }

            }
        }
    }
}
