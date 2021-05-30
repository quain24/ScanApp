﻿using ScanApp.Common.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;

namespace ScanApp.Components.Common.AltTableTest
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
                return value is null || value <= To.Value.Date;
            }
            if (To is null)
            {
                return value is null || value >= From.Value.Date;
            }

            return value is not null && value >= From.Value.Date && value <= To.Value.Date;
        }
    }
}