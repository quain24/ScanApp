using FluentValidation;
using MudBlazor;

namespace ScanApp.Components.Common.Table
{
    public interface IColumnBuilder<T>
    {
        IColumnBuilder<T> UnderName(string name);

        IColumnBuilder<T> ValidateUsing(IValidator validator);

        IColumnBuilder<T> FormatAs(FieldType type);

        IColumnBuilder<T> ConverterUsing<TType>(Converter<TType> converter);

        IColumnBuilder<T> AsReadOnly();

        IColumnBuilder<T> DisableGrouping();

        IColumnBuilder<T> DisableFiltering();

        ColumnConfig<T> Create();
    }
}