using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Pivot
{
    /// <summary>
    /// Pivot measure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PivotMeasure<T> {

        public string Format { get; set; } = "";
        /// <summary>
        /// Create a Measure list that calculates the total
        /// </summary>
        /// <param name="PropertyNames"></param>
        /// <returns></returns>
        public static List<PivotMeasure<T>> Build(params string[] PropertyNames) => PropertyNames.Select(p => new PivotMeasure<T>(p)).ToList();
        /// <summary>
        /// Create a measure that calculates the total
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Sum(string propertyName, Func<T, decimal> valueGetter = null, string title = null) => new PivotMeasure<T>(propertyName, valueGetter, title) { aggregate = aggregate_sum };
        /// <summary>
        /// Create a measure to calculate the count
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Count(string propertyName, Func<T, decimal> valueGetter = null, string title = null) => new PivotMeasure<T>(propertyName, valueGetter, title) { aggregate = aggregate_count };
        /// <summary>
        /// Create a measure to calculate the average
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Average(string propertyName, Func<T, decimal> valueGetter = null, string title = null) => new PivotMeasure<T>(propertyName, valueGetter, title) { aggregate = aggregate_average };
        /// <summary>
        /// Create a measure to calculate the maximum
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Max(string propertyName, Func<T, decimal> valueGetter = null, string title = null) => new PivotMeasure<T>(propertyName, valueGetter, title) { aggregate = aggregate_max };
        /// <summary>
        /// Create a measure to calculate the minimum
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Min(string propertyName, Func<T, decimal> valueGetter = null, string title = null) => new PivotMeasure<T>(propertyName, valueGetter, title) { aggregate = aggregate_min };
        /// <summary>
        /// Property Name of T
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// delegate get value of T
        /// </summary>
        internal Func<T, decimal> ValueGetter { get; set; }

        public PivotMeasure(string propertyName, Func<T, decimal> valueGetter = null, string title = null)
        {
            this.PropertyName = propertyName;
            ValueGetter = valueGetter;
            Title = title is null ? propertyName : title;
        }
        /// <summary>
        /// delegate aggregate 
        /// </summary>
        public Func<IEnumerable<decimal>, decimal> aggregate { get; set; } = aggregate_sum;

        #region aggregate functions
        private static Func<IEnumerable<decimal>, decimal> aggregate_sum = (list) => list.Any()?list.Sum():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_count = (list) => list.Any() ? list.Count():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_average = (list) => list.Any() ? list.Average():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_max = (list) => list.Any() ? list.Max():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_min = (list) => list.Any() ? list.Min():0;
        #endregion
    }
   

}
