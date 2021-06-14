using FluentValidation;
using MudBlazor;
using System;
using System.Linq.Expressions;

namespace ScanApp.Components.Common.Table
{
    /// <summary>
    /// Provides an easy way to create custom <see cref="ColumnConfig{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type that will be configured by built <see cref="ColumnConfig{T}"/>.</typeparam>
    public class ColumnBuilder<T> : IColumnBuilder<T>
    {
        private readonly Expression<Func<T, dynamic>> _target;
        private string _name;
        private IValidator _validator;
        private dynamic _converter;
        private FieldType _type;
        private bool _isFilterable = true;
        private bool _isEditable = true;
        private bool _isGroupable = true;

        private ColumnBuilder(Expression<Func<T, dynamic>> target)
        {
            _target = target;
        }

        /// <summary>
        /// Starting point for creation of new <see cref="ColumnConfig{T}"/> for given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">Built <see cref="ColumnConfig{T}"/> will point to this.</param>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public static IColumnBuilder<T> For(Expression<Func<T, dynamic>> target)
        {
            return new ColumnBuilder<T>(target);
        }

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'read-only'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> AsReadOnly()
        {
            _isEditable = false;
            return this;
        }

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'non-groupable'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> DisableGrouping()
        {
            _isGroupable = false;
            return this;
        }

        /// <summary>
        /// Set <see cref="ColumnConfig{T}"/> being configured as 'non-filterable'.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> DisableFiltering()
        {
            _isFilterable = false;
            return this;
        }

        /// <summary>
        /// Set name under which built <see cref="ColumnConfig{T}"/> will be displayed.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> UnderName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Set validator that will be used to validate objects pointed to by built <see cref="ColumnConfig{T}"/> <c>target</c>.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> ValidateUsing(IValidator validator)
        {
            _validator = validator;
            return this;
        }

        /// <summary>
        /// Set field format that will be used when build <see cref="ColumnConfig{T}"/> will be displayed.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> FormatAs(FieldType type)
        {
            _type = type;
            return this;
        }

        /// <summary>
        /// Set converter that will be used to translate from and to a <see cref="SCTable{TTableType}"/> display-friendly format.
        /// </summary>
        /// <returns>Instance of <see cref="IColumnBuilder{T}"/> for further configuration.</returns>
        public IColumnBuilder<T> ConvertUsing<TType>(Converter<TType> converter)
        {
            _converter = converter;
            return this;
        }

        /// <summary>
        /// Creates new configured <see cref="ColumnConfig{T}"/>.
        /// </summary>
        /// <returns><see cref="ColumnConfig{T}"/> configured by this instance of builder.</returns>
        public ColumnConfig<T> Build()
        {
            var config = new ColumnConfig<T>(_target, _name, _type, _validator)
            {
                IsEditable = _isEditable,
                IsGroupable = _isGroupable,
                IsFilterable = _isFilterable
            };

            if (_converter is not null)
                config.AssignConverter(_converter);
            return config;
        }
    }
}