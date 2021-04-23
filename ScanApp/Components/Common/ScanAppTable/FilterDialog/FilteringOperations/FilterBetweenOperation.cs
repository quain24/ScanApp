using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanApp.Components.Common.ScanAppTable.Extensions;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterBetweenOperation : IFilteringOperation
    {
        private string _propertyName;
        private int? _from;
        private int? _to;

        public FilterBetweenOperation(string propertyName, int? from, int? to)
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
            return items.FilterBetween(_propertyName, _from, _to);
        }
    }
}
