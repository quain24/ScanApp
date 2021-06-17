using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanApp.Components.Common.Table.Utilities
{
    /// <summary>
    /// Used to provide custom column styles for <see cref="SCTable{TTableType}"/> in form of <see cref="MarkupString"/>.
    /// </summary>
    /// <typeparam name="T">Type of item used in styled <see cref="SCTable{TTableType}"/>.</typeparam>
    public class ColumnStyleBuilder<T>
    {
        /// <summary>
        /// Creates <see cref="MarkupString"/> containing all provided column styles (if any) to be applied by <see cref="SCTable{TTableType}"/>.
        /// </summary>
        /// <param name="configs">Column configuration files used to create custom styles.</param>
        /// <returns>markup string containing prepared column styles.</returns>
        public MarkupString BuildUsing(IList<ColumnConfig<T>> configs)
        {
            if (configs?.Any(c => c is null) ?? true)
            {
                throw new ArgumentNullException(nameof(configs), $"{configs} collections is null or one of configs is null.");
            }

            var htmlBuilder = new StringBuilder();

            foreach (var columnConfig in configs)
            {
                if (string.IsNullOrEmpty(columnConfig.ColumnStyle))
                {
                    htmlBuilder.Append("<col />");
                }
                else
                {
                    htmlBuilder.Append("<col style=\"").Append(columnConfig.ColumnStyle).Append("\" />");
                }

                if (columnConfig != configs.Last())
                    htmlBuilder.Append('\n');
            }
            return new MarkupString(htmlBuilder.ToString());
        }
    }
}