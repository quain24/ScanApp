using System;

namespace ScanApp.Components.Common.Table.Utilities
{
    public class InBetweenInclusiveFilterDateOnly<T> : InBetweenInclusiveFilter<T>
    {
        public InBetweenInclusiveFilterDateOnly(ColumnConfig<T> columnConfig, DateTime? @from, DateTime? to) : base(columnConfig, @from, to)
        {
        }

        protected override bool CheckValue(T item)
        {
            dynamic value = ColumnConfig.GetValueFrom(item);

            if (From is null)
            {
                return value is null || value.Date <= To.Date;
            }
            if (To is null)
            {
                return value is null || value.Date >= From.Date;
            }

            return value is not null && (value.Date >= From.Date && value.Date <= To.Date);
        }
    }
}