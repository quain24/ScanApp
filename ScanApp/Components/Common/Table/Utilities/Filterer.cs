using SharedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Components.Common.Table.Utilities
{
    /// <summary>
    /// Provides filtration functionality on given collection of items in <see cref="SCTable{TTableType}"/>.
    /// </summary>
    public static class Filterer
    {
        /// <summary>
        /// Filters given <paramref name="collection"/> using provided <paramref name="filters"/>.
        /// </summary>
        /// <typeparam name="T">Type of item used in the <paramref name="collection"/> and <paramref name="filters"/>.</typeparam>
        /// <param name="collection">Collection of <typeparamref name="T"/> to be filtered.</param>
        /// <param name="filters">One or more filters used to filter out given <paramref name="collection"/>.</param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> containing <typeparam name="T"> items of which every one complies with all given <paramref name="filters"/>.</typeparam>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">One of <paramref name="filters"/> is <see langword="null"/>.</exception>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> collection, params IFilter<T>[] filters)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            if (filters.IsNullOrEmpty()) return collection;
            if (filters.Any(f => f is null)) throw new ArgumentNullException(nameof(collection), "one of given filters is null.");

            return filters.Length == 1
                ? filters[0].Run(collection)
                : collection.AsParallel().Where(item => filters.All(f => f.Check(item)));
        }

        ///<inheritdoc cref="Filter{T}(System.Collections.Generic.IEnumerable{T},ScanApp.Components.Common.Table.Utilities.IFilter{T}[])"/>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> collection, IEnumerable<IFilter<T>> filters)
        {
            return Filter(collection, filters?.ToArray());
        }
    }
}