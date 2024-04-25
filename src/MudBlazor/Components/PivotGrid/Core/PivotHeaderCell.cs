// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Pivot
{

    /// <summary>
    /// pivot header cell
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PivotHeaderCell<T>
    {
        internal PivotHeaderCell(T leadObjet, PivotColumn<T> column)
        {
            LeadObject = leadObjet;
            Column = column;
        }
        /// <summary>
        /// cell value
        /// </summary>
        public string Value => Column.ValueGetter(LeadObject);
        /// <summary>
        /// cell display title
        /// </summary>
        public string Title => Column.TitleGetter(LeadObject);
        /// <summary>
        /// T contained in this cell
        /// </summary>
        internal T LeadObject { get; set; }
        /// <summary>
        /// Parent cell of Cell-Tree hierarchy
        /// </summary>
        internal PivotHeaderCell<T> Parent { get; set; }
        /// <summary>
        /// Child cells of Cell-Tree hierarchy
        /// </summary>
        internal List<PivotHeaderCell<T>> __Children { get; private set; } = new List<PivotHeaderCell<T>>();
        /// <summary>
        /// Child cells of Cell-Tree hierarchy
        /// </summary>
        public IEnumerable<PivotHeaderCell<T>> Children => __Children.ToList();
        /// <summary>
        /// Column what contain this cell
        /// </summary>
        public PivotColumn<T> Column { get; internal set; }

        internal bool HasChildren
        {
            get => __Children.Any();
        }

        /// <summary>
        /// lowest level cells count of progeny
        /// </summary>
        public int CountLeaves
        {
            get
            {
                if (Column.LowerColumn == null)
                {
                    return 1;
                }
                else
                {
                    return __Children.Sum(c => c.CountLeaves);
                }
            }
        }
        /// <summary>
        /// Path of tree
        /// </summary>
        public IEnumerable<PivotHeaderCell<T>> Path
        {
            get
            {
                var path = new List<PivotHeaderCell<T>>();
                var current = this;
                while (current != null)
                {
                    path.Add(current);
                    current = current.Parent;
                }
                path.Reverse();
                return path;
            }
        }
        /// <summary>
        /// Is First Child Cell in Parent
        /// </summary>
        public bool IsFirstChild => Parent == null ? true : Parent.Children.First() == this;
        /// <summary>
        /// Is Last Child Cell in Parent
        /// </summary>
        public bool IsLastChild => Parent == null ? true : Parent.Children.Last() == this;

    }
}
