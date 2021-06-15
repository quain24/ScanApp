using FluentValidation;
using MudBlazor;
using System;
using System.Linq.Expressions;

namespace ScanApp.Components.Common.Table
{
    /// <inheritdoc cref="IColumnBuilder{T}"/>
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
        private string _columnStyle;

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

        public IColumnBuilder<T> AsReadOnly()
        {
            _isEditable = false;
            return this;
        }

        public IColumnBuilder<T> DisableGrouping()
        {
            _isGroupable = false;
            return this;
        }

        public IColumnBuilder<T> DisableFiltering()
        {
            _isFilterable = false;
            return this;
        }

        public IColumnBuilder<T> UnderName(string name)
        {
            _name = name;
            return this;
        }

        public IColumnBuilder<T> ValidateUsing(IValidator validator)
        {
            _validator = validator;
            return this;
        }

        public IColumnBuilder<T> FormatAs(FieldType type)
        {
            _type = type;
            return this;
        }

        public IColumnBuilder<T> ConvertUsing<TType>(Converter<TType> converter)
        {
            _converter = converter;
            return this;
        }

        public IColumnBuilder<T> Style(string cssColumnStyle)
        {
            _columnStyle = cssColumnStyle;
            return this;
        }

        public ColumnConfig<T> Build()
        {
            var config = new ColumnConfig<T>(_target, _name, _type, _validator)
            {
                IsEditable = _isEditable,
                IsGroupable = _isGroupable,
                IsFilterable = _isFilterable,
                ColumnStyle = _columnStyle
            };

            if (_converter is not null)
                config.AssignConverter(_converter);
            return config;
        }
    }
}