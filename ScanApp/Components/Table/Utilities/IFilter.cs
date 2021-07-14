using System.Collections.Generic;

namespace ScanApp.Components.Table.Utilities
{
    /// <summary>
    /// Provides a way to filter data using provided <see cref="ColumnConfig{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of data being filtered.</typeparam>
    public interface IFilter<T>
    {
        /// <summary>
        /// Gets <see cref="ColumnConfig{T}"/> that points to specific item inside <typeparamref name="T"/> by which collection will be filtered.
        /// </summary>
        ColumnConfig<T> ColumnConfig { get; }

        /// <summary>
        /// Checks if <paramref name="item"/> meets the conditions of this <see cref="IFilter{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns><see langword="true"/> if <paramref name="item"/> met conditions of this <see cref="IFilter{T}"/>; Otherwise <see langword="false"/>.</returns>
        bool Check(T item);

        /// <summary>
        /// Checks which of elements in <paramref name="source"/> meets conditions of this <see cref="IFilter{T}"/>.
        /// </summary>
        /// <param name="source">Collection to be filtered.</param>
        /// <returns>A collection containing items that met conditions of this <see cref="IFilter{T}"/>.</returns>
        IEnumerable<T> Run(IEnumerable<T> source);
    }
}