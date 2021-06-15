using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanApp.Components.Common.Table.Utilities
{
    public class ColumnStyleBuilder<T>
    {
        private readonly IEnumerable<ColumnConfig<T>> _configs;

        public ColumnStyleBuilder(IEnumerable<ColumnConfig<T>> configs)
        {
            if (configs?.Any(c => c is null) ?? true)
            {
                throw new ArgumentNullException(nameof(configs), $"{configs} collections is null or one of configs is null.");
            }

            _configs = configs;
        }

        public MarkupString Build()
        {
            var htmlBuilder = new StringBuilder();

            foreach (var columnConfig in _configs)
            {
                if (string.IsNullOrEmpty(columnConfig.ColumnStyle))
                {
                    htmlBuilder.Append("<col />");
                }
                else
                {
                    htmlBuilder.Append("<col style=\"").Append(columnConfig.ColumnStyle).Append("\" />");
                }

                if (columnConfig != _configs.Last())
                    htmlBuilder.Append('\n');
            }
            return new MarkupString(htmlBuilder.ToString());
        }
    }
}