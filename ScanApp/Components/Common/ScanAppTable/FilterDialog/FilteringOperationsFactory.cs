using System;
using System.Collections.Generic;
using System.Reflection;
using ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog
{
    public class FilteringOperationsFactory
    {
        public FilteringOperationsFactory(PropertyInfo[] properties, int?[] from, int?[] to, string[] contains, DateTime?[] fromDate, DateTime?[] toDate)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties), "Properties argument is null. Cannot run filtering without properties.");
            From = from;
            To = to;
            Contains = contains;
            FromDate = fromDate;
            ToDate = toDate;
            Length = properties.Length;
        }

        private int?[] From { get; set; }
        private int?[] To { get; set; }
        private string[] Contains { get; set; }
        private DateTime?[] FromDate { get; set; }
        private DateTime?[] ToDate { get; set; }
        private PropertyInfo[] Properties { get; set; }
        private int Length { get; set; }
        private List<IFilteringOperation> FilteringOperations { get; set; } = new List<IFilteringOperation>();

        public List<IFilteringOperation> CreateOperations()
        {
            for (int i = 0; i < Length; i++)
            {
                if (ArgumentsAreValid(From[i], To[i]))
                {
                    FilteringOperations.Add(new FilterBetweenOperation(Properties[i].Name, From[i], To[i]));
                }
            }

            for (int i = 0; i < Length; i++)
            {
                if (ArgumentsAreValid(FromDate[i], ToDate[i]))
                {
                    FilteringOperations.Add(new FilteringBetweenDatesOperation(Properties[i].Name, FromDate[i], ToDate[i]));
                }
            }

            for (int i = 0; i < Length; i++)
            {
                if (!String.IsNullOrWhiteSpace(Contains[i]))
                {
                    FilteringOperations.Add(new FilterContainsOperation(Properties[i].Name, Contains[i]));
                }
            }

            return FilteringOperations;
        }

        private bool ArgumentsAreValid(int? from, int? to)
        {
            if (from is null && to is null)
            {
                return false;
            }

            return true;
        }

        private bool ArgumentsAreValid(DateTime? from, DateTime? to)
        {
            if (from is null && to is null)
            {
                return false;
            }

            return true;
        }
    }
}