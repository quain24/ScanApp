using FluentValidation;
using MudBlazor;
using System;
using System.Linq.Expressions;

namespace ScanApp.Components.Common.Table
{
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

        public IColumnBuilder<T> ConverterUsing<TType>(Converter<TType> converter)
        {
            _converter = converter;
            return this;
        }

        public ColumnConfig<T> Create()
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