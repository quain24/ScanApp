using System;
using ScanApp.Common.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.Table.Utilities
{
    public class InBetweenInclusiveFilterTimeOnly<T> : InBetweenInclusiveFilter<T>
    {
        public InBetweenInclusiveFilterTimeOnly(ColumnConfig<T> columnConfig, TimeSpan? @from, TimeSpan? to) : base(columnConfig, @from, to)
        {
        }

        protected override bool CheckValue(T item)
        {
            var value = ColumnConfig.GetValueFrom(item);
            TimeSpan? compareTo = null;
            
            if (value is TimeSpan)
                compareTo = value;
            else if (value is DateTime)
                compareTo = value.TimeOfDay;

            if (From is null)
            {
                return compareTo is null || compareTo <= To;
            }
            if (To is null)
            {
                return compareTo is null || compareTo >= From;
            }

            return compareTo is not null && compareTo >= From && compareTo <= To;
        }
    }
}