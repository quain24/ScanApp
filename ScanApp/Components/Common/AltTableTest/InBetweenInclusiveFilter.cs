using ScanApp.Common.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Components.Common.AltTableTest
{
    public class InBetweenInclusiveFilter<T> : IFilter<T>
    {
        protected readonly dynamic From;
        protected readonly dynamic To;
        private readonly string _message = $"argument can be either a {nameof(DateTime)}, {nameof(DateTimeOffset)}, {nameof(TimeSpan)}, numeric or null.";
        private readonly Func<T, bool> _checkDelegate;

        public ColumnConfig<T> ColumnConfig { get; }

        public InBetweenInclusiveFilter(ColumnConfig<T> columnConfig, dynamic from, dynamic to)
        {
            if (CanBeUsed(from) is false)
                throw new ArgumentException($"'{nameof(from)}' {_message}", nameof(from));

            if (CanBeUsed(to) is false)
                throw new ArgumentException($"'{nameof(to)}' {_message}", nameof(to));

            if (TypesMatch(from, to) is false)
                throw new ArgumentException($"'{nameof(from)}' type and '{nameof(to)}' type are not matched.");

            ColumnConfig = columnConfig ?? throw new ArgumentNullException(nameof(columnConfig));
            From = from;
            To = to;

            if (CanBeUsed(columnConfig.PropertyType) is false)
            {
                throw new ArgumentException($"Type of property being filtered ({columnConfig.PropertyType.Name})" +
                                            $" stored in {nameof(columnConfig)} is not compatible with {nameof(InBetweenInclusiveFilter<T>)} sorting algorithm.");
            }

            if (TypesMatch(from, columnConfig.PropertyType) is false || TypesMatch(to, columnConfig.PropertyType))
            {
                throw new ArgumentException($"Type of property being filtered ({columnConfig.PropertyType.Name})" +
                                            $" is not the same as types of '{nameof(from)}' or/and '{nameof(to)}' parameters.");
            }

            _checkDelegate = To is null && From is null ? _ => true : CheckValue;
        }

        private static bool CanBeUsed(dynamic value)
        {
            return value is DateTime or TimeSpan or DateTimeOffset ||
                   (((Type)value?.GetType())?.IsNumeric() ?? true);
        }

        private static bool TypesMatch(dynamic one, dynamic two)
        {
            if (one is null || two is null) return true;
            return (one is Type ? one : (Type)one.GetType()) == (two is Type ? two : (Type)two.GetType());
        }

        public virtual bool Check(T item) => _checkDelegate(item);

        protected virtual bool CheckValue(T item)
        {
            var value = ColumnConfig.GetValueFrom(item);

            if (From is null)
            {
                return value is null || value <= To;
            }
            if (To is null)
            {
                return value is null || value >= From;
            }

            return value is not null && value >= From && value <= To;
        }

        public IEnumerable<T> Run(IEnumerable<T> source)
        {
            if (From is null && To is null)
                return source.ToArray();

            return source.Where(CheckValue);
        }
    }
}