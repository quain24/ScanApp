using FluentValidation;
using MudBlazor;
using System.Collections.Generic;

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
        /// Set validator that will be used to validate objects pointed to by built <see cref="ColumnConfig{T}"/> <c>target</c>.<para/>
        /// Provided <see cref="IValidator"/> <b>must</b> be compatible with target item pointed to by this instance of <see cref="ColumnConfig{T}"/>.
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
        /// Parent <see cref="SCTable{TTableType}"/> will be allowed to edit item pointed to by this instance of <see cref="ColumnConfig{T}"/>.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> Editable();

        /// <summary>
        /// Parent <see cref="SCTable{TTableType}"/> will be allowed to group by this <see cref="ColumnConfig{T}"/>.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> Groupable();

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'non-filterable'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> DisableFiltering();

        /// <summary>
        /// Sets up CSS style for column configured by <see cref="ColumnConfig{T}"/> being configured.
        /// </summary>
        /// <param name="cssColumnStyle">String representation of CSS style to be applied.</param>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> ColumnStyle(string cssColumnStyle);

        /// <summary>
        /// Limits possible input values for <see cref="ColumnConfig{T}"/> target for when <typeparamref name="T"/> item is being edited or added.
        /// </summary>
        /// <param name="values">Allowed values - user will be able to choose from this set only when editing or adding value inside <typeparamref name="T"/> item
        /// targeted by <see cref="ColumnConfig{T}"/>being configured.</param>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        IColumnBuilder<T> LimitValuesTo(IEnumerable<object> values);

        /// <summary>
        /// Creates new configured <see cref="ColumnConfig{T}"/>.
        /// </summary>
        /// <returns><see cref="ColumnConfig{T}"/> configured by this instance of builder.</returns>
        ColumnConfig<T> Build();
    }
}