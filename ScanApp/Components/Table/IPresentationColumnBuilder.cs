namespace ScanApp.Components.Table
{
    public interface IPresentationColumnBuilder<T>
    {
        /// <inheritdoc cref="IColumnBuilder{T}.ColumnStyle"/>
        /// <param name="cssColumnStyle">String representation of SCC style to be applied.</param>
        /// <returns>Instance of <see cref="IPresentationColumnBuilder{T}"/> for further configuration.</returns>
        IPresentationColumnBuilder<T> ColumnStyle(string cssColumnStyle);

        /// <summary>
        /// Creates new <see cref="ColumnConfig{T}"/> configured as presenter.
        /// </summary>
        /// <returns><see cref="ColumnConfig{T}"/> configured by this instance of builder.</returns>
        ColumnConfig<T> Build();
    }
}