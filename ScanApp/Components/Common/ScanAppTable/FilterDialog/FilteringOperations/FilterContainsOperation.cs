using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanApp.Components.Common.ScanAppTable.Extensions;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterContainsOperation : IFilteringOperation
    {
        private string _propertyName;
        private string _contains;

        public FilterContainsOperation(string propertyName, string contains)
        {
            _propertyName = propertyName;
            _contains = contains;
        }

        public string PropertyName
        {
            get => _propertyName;
            set => _propertyName = value;
        }

        public IEnumerable<T> Run<T>(IEnumerable<T> items)
        {
            return items.FilterContains(_propertyName, _contains);
        }
    }
}
