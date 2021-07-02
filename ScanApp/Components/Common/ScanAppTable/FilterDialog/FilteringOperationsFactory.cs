using ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations;
using System;
using System.Collections.Generic;
using System.Reflection;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog
{
    public class FilteringOperationsFactory<TItem>
    {
        public FilteringOperationsFactory(List<ColumnConfig<TItem>> columnConfigs, int?[] from, int?[] to, string[] contains, DateTime?[] fromDate, DateTime?[] toDate,
                decimal?[] fromDecimal, decimal?[] toDecimal)
        {
            From = from;
            To = to;
            Contains = contains;
            FromDate = fromDate;
            ToDate = toDate;
            FromDecimal = fromDecimal;
            ToDecimal = toDecimal;
            ColumnConfigs = columnConfigs ?? throw new ArgumentNullException(nameof(ColumnConfigs), 
                "ColumnConfigs argument is null. Cannot run filtering without ColumnConfigs.");
            Length = ColumnConfigs.Count;
        }

        private List<ColumnConfig<TItem>> ColumnConfigs { get; set; }
        private int?[] From { get; set; }
        private int?[] To { get; set; }
        private string[] Contains { get; set; }
        private DateTime?[] FromDate { get; set; }
        private DateTime?[] ToDate { get; set; }
        private decimal?[] FromDecimal { get; set; }
        private decimal?[] ToDecimal { get; set; }
        private int Length { get; set; }
        private List<IFilteringOperation<TItem>> FilteringOperations { get; set; } = new List<IFilteringOperation<TItem>>();

        public List<IFilteringOperation<TItem>> CreateOperations()
        {
            for (int i = 0; i < Length; i++)
            {
                if (ArgumentsAreValid(From[i], To[i]))
                {
                    FilteringOperations.Add(new FilterBetweenOperation<TItem>(ColumnConfigs[i], From[i], To[i]));
                }

                if (ArgumentsAreValid(FromDate[i], ToDate[i]))
                {
                    FilteringOperations.Add(
                        new FilteringBetweenDatesOperation<TItem>(ColumnConfigs[i], FromDate[i], ToDate[i]));
                }

                if (!String.IsNullOrWhiteSpace(Contains[i]))
                {
                    FilteringOperations.Add(new FilterContainsOperation<TItem>(ColumnConfigs[i], Contains[i]));
                }

                if (ArgumentsAreValid(FromDecimal[i], ToDecimal[i]))
                {
                    FilteringOperations.Add(
                        new FilterBetweenDecimalsOperation<TItem>(ColumnConfigs[i], FromDecimal[i], ToDecimal[i]));
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