using ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog
{
    public class FilteringOperationsFactory
    {
        public FilteringOperationsFactory(PropertyInfo[] properties, int?[] from, int?[] to, string[] contains, DateTime?[] fromDate, DateTime?[] toDate,
                decimal?[] fromDecimal, decimal?[] toDecimal)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties), "Properties argument is null. Cannot run filtering without properties.");
            From = from;
            To = to;
            Contains = contains;
            FromDate = fromDate;
            ToDate = toDate;
            FromDecimal = fromDecimal;
            ToDecimal = toDecimal;
            Length = properties.Length;
        }

        private int?[] From { get; set; }
        private int?[] To { get; set; }
        private string[] Contains { get; set; }
        private DateTime?[] FromDate { get; set; }
        private DateTime?[] ToDate { get; set; }
        private decimal?[] FromDecimal { get; set; }
        private decimal?[] ToDecimal { get; set; }
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

                if (ArgumentsAreValid(FromDate[i], ToDate[i]))
                {
                    FilteringOperations.Add(
                        new FilteringBetweenDatesOperation(Properties[i].Name, FromDate[i], ToDate[i]));
                }

                if (!String.IsNullOrWhiteSpace(Contains[i]))
                {
                    FilteringOperations.Add(new FilterContainsOperation(Properties[i].Name, Contains[i]));
                }

                if (ArgumentsAreValid(FromDecimal[i], ToDecimal[i]))
                {
                    FilteringOperations.Add(
                        new FilterBetweenDecimalsOperation(Properties[i].Name, FromDecimal[i], ToDecimal[i]));
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

        private bool ArgumentsAreValid(decimal? from, decimal? to)
        {
            if (from is null && to is null)
            {
                return false;
            }

            return true;
        }
    }
}