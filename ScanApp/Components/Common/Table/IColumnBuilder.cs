using FluentValidation;
using MudBlazor;

namespace ScanApp.Components.Common.Table
{
    /// <summary>
    /// Provides an easy way to create custom <see cref="ColumnConfig{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type that will be configured by built <see cref="ColumnConfig{T}"/>.</typeparam>
    public interface IColumnBuilder<T>
    {
        /// <summary>
        /// Set name under which built <see cref="ColumnConfig{T}"/> will be displayed.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> UnderName(string name);

        /// <summary>
        /// Set validator that will be used to validate objects pointed to by built <see cref="ColumnConfig{T}"/> <c>target</c>.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> ValidateUsing(IValidator validator);

        /// <summary>
        /// Set field format that will be used when build <see cref="ColumnConfig{T}"/> will be displayed.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> FormatAs(FieldType type);

        /// <summary>
        /// Set converter that will be used to translate from and to a <see cref="SCTable{TTableType}"/> display-friendly format.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> ConvertUsing<TType>(Converter<TType> converter);

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'read-only'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> AsReadOnly();

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'non-groupable'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> DisableGrouping();

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'non-filterable'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> DisableFiltering();

        /// <summary>
        /// Creates new configured <see cref="ColumnConfig{T}"/>.
        /// </summary>
        /// <returns><see cref="ColumnConfig{T}"/> configured by this instance of builder.</returns>
        ColumnConfig<T> Build();
    }
}