using ScanApp.Components.Common.ScanAppTable.Extensions;
using System;
using System.Collections.Generic;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilteringBetweenDatesOperation : IFilteringOperation
    {
        private string _propertyName;
        private DateTime? _fromDate;
        private DateTime? _toDate;

        public FilteringBetweenDatesOperation(string propertyName, DateTime? fromDate, DateTime? toDate)
        {
            _propertyName = propertyName;
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public string PropertyName
        {
            get => _propertyName;
            set => _propertyName = value;
        }

        public IEnumerable<T> Run<T>(IEnumerable<T> items)
        {
            return items.FilterBetweenDates(_propertyName, _fromDate, _toDate);
        }
    }
}