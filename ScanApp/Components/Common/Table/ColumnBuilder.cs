using FluentValidation;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ScanApp.Components.Common.Table
{
    /// <inheritdoc cref="IColumnBuilder{T}"/>
    /// <typeparam name="T">Type that will be configured by built <see cref="ColumnConfig{T}"/>.</typeparam>
    public class ColumnBuilder<T> : IColumnBuilder<T>, IPresentationColumnBuilder<T>
    {
        private readonly Expression<Func<T, dynamic>> _target;
        private string _name;
        private dynamic _validator;
        private dynamic _converter;
        private FieldType _type;
        private bool _isFilterable = true;
        private bool _isEditable;
        private bool _isGroupable;
        private bool _isVisible = true;
        private string _columnStyle;
        private dynamic _allowedValues;

        private ColumnBuilder(Expression<Func<T, dynamic>> target = null)
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
        /// Immediately builds a 'presenter' <see cref="ColumnConfig{T}"/> to be used with accompanying <see cref="SCColumn{T}"/> component.
        /// </summary>
        /// <param name="displayName">name used as a column name for this config.</param>
        /// <returns>A <see cref="ColumnConfig{T}"/> configured as a 'presenter'.</returns>
        public static IPresentationColumnBuilder<T> ForPresentation(string displayName) => new ColumnBuilder<T> { _name = displayName };

        public IColumnBuilder<T> Editable()
        {
            _isEditable = true;
            return this;
        }

        public IColumnBuilder<T> Groupable()
        {
            _isGroupable = true;
            return this;
        }

        public IColumnBuilder<T> Invisible()
        {
            _isVisible = false;
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

        public IColumnBuilder<T> ValidateUsing<TType>(IValidator<TType> validator)
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

        public IColumnBuilder<T> ColumnStyle(string cssColumnStyle)
        {
            _columnStyle = cssColumnStyle;
            return this;
        }

        IPresentationColumnBuilder<T> IPresentationColumnBuilder<T>.ColumnStyle(string cssColumnStyle)
        {
            _columnStyle = cssColumnStyle;
            return this;
        }

        public IColumnBuilder<T> LimitValuesTo(IEnumerable<dynamic> values)
        {
            _allowedValues = values;
            return this;
        }

        public ColumnConfig<T> Build()
        {
            ColumnConfig<T> config;
            if (_target is null)
            {
                config = new ColumnConfig<T>(_name)
                {
                    ColumnStyle = _columnStyle
                };
            }
            else
            {
                config = new ColumnConfig<T>(_target, _name, _type)
                {
                    IsEditable = _isEditable,
                    IsGroupable = _isGroupable,
                    IsFilterable = _isFilterable,
                    ColumnStyle = _columnStyle,
                    IsVisible = _isVisible
                };
            }

            if (_converter is not null)
                config.AssignConverter(_converter);
            if (_validator is not null)
                config.AssignValidator(_validator);
            if (_allowedValues is not null)
                config.LimitAcceptedValuesTo(_allowedValues);
            return config;
        }
    }
}