using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Pivot 
{
    /// <summary>
    /// Column(Row) definition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PivotColumn<T> {

        /// <summary>
        /// Create PivotColumn list to aggregate by PropertyNames
        /// </summary>
        /// <param name="PropertyNames">Property Name of T</param>
        /// <param name="options">rendering options</param>
        /// <returns></returns>
        public static List<PivotColumn<T>> Build(PivotAxisRenderOption options, params string[] PropertyNames) => PropertyNames.Select(p => new PivotColumn<T>(p, options: options)).ToList();
        /// <summary>
        /// Create PivotColumn list to aggregate by PropertyNames
        /// </summary>
        /// <param name="PropertyNames">Property Name of T</param>
        /// <returns></returns>
        public static List<PivotColumn<T>> Build(params string[] PropertyNames) => PropertyNames.Select(p => new PivotColumn<T>(p)).ToList();
        
        /// <summary>
        /// Property Name of T
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Column listing Order
        /// </summary>
        public PivotOrder Order { get; set; } = PivotOrder.Ascending;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="titleGetter">delegate to Get title Action</param>
        /// <param name="valueGetter">delegate to Get value Action</param>
        /// <param name="options">rendering options</param>
        public PivotColumn(string PropertyName, Func<T, string> titleGetter = null, Func<T, dynamic> valueGetter = null, PivotAxisRenderOption options = null) {
            Options = options is null ? new PivotAxisRenderOption() : options;
            this.PropertyName = PropertyName;
            ValueGetter = valueGetter;
            TitleGetter = titleGetter;
        }
        
        internal List<PivotHeaderCell<T>> Cells { get; } = new List<PivotHeaderCell<T>>();

        /// <summary>
        /// All PivotHeaderCells at this level
        /// </summary>
        public IEnumerable<PivotHeaderCell<T>> Items => Cells.ToList();
        /// <summary>
        /// delegate get Value of T
        /// </summary>
        internal Func<T, dynamic> ValueGetter { get; set; }
        /// delegate get Title of T
        internal Func<T, string> TitleGetter { get; set; }

        /// <summary>
        /// Compare T of List and T of LeadObject
        /// </summary>
        internal bool EqualsExpression(T obj, T param) {
            return ValueGetter(obj) == ValueGetter(param);
        }
        /// <summary>
        /// Order/Group key getter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal dynamic KeySelector(T obj) => ValueGetter(obj);
        /// <summary>
        /// Higher level column
        /// </summary>
        internal PivotColumn<T> HigherColumn { get; set; }
        /// <summary>
        /// lower level column
        /// </summary>
        internal PivotColumn<T> LowerColumn { get; set; }

        public PivotAxisRenderOption Options { get; set; }
    }

}
