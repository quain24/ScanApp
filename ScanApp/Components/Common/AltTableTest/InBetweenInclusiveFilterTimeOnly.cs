using ScanApp.Common.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;

namespace ScanApp.Components.Common.AltTableTest
{
    public class InBetweenInclusiveFilterTimeOnly<T> : InBetweenInclusiveFilter<T>
    {
        public InBetweenInclusiveFilterTimeOnly(ColumnConfig<T> columnConfig, TimeSpan? @from, TimeSpan? to) : base(columnConfig, @from, to)
        {
        }

        protected override bool CheckValue(T item)
        {
            var value = ColumnConfig.GetValueFrom(item);
            TimeSpan? compareTo;

            if (value is TimeSpan)
                compareTo = value;
            else if (Nullable.GetUnderlyingType(value) is DateTime)
                compareTo = value.HasValue ? value.Value.TimeOfDay : null;
            else
                compareTo = value.TimeOfDay;

            if (From is null)
            {
                return compareTo is null || compareTo <= To.Value.TimeOfDay;
            }
            if (To is null)
            {
                return compareTo is null || compareTo >= From.Value.TimeOfDay;
            }

            return compareTo is not null && compareTo >= From.Value.TimeOfDay && compareTo <= To.Value.TimeOfDay;
        }
    }
}