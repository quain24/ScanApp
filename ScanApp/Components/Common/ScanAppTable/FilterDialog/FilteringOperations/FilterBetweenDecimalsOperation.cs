using ScanApp.Components.Common.ScanAppTable.Extensions;
using System.Collections.Generic;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterBetweenDecimalsOperation : IFilteringOperation
    {
        private string _propertyName;
        private decimal? _from;
        private decimal? _to;

        public FilterBetweenDecimalsOperation(string propertyName, decimal? from, decimal? to)
        {
            _propertyName = propertyName;
            _from = from;
            _to = to;
        }

        public string PropertyName
        {
            get => _propertyName;
            set => _propertyName = value;
        }

        public IEnumerable<T> Run<T>(IEnumerable<T> items)
        {
            return items.FilterBetweenDecimals(_propertyName, _from, _to);
        }
    }
}