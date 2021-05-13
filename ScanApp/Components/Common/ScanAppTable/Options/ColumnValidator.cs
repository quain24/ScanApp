using ScanApp.Common;
using System;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnValidator<T>
    {
        private Type _type { get; set; }
        public string PropertyName { get; init; }
        public FluentValidationWrapper<T> Validator { get; init; }

        public ColumnValidator(string propertyName, FluentValidationWrapper<T> validator)
        {
            _type = typeof(T);
            if (Nullable.GetUnderlyingType(_type) == null && _type != typeof(string))
            {
                throw new ArgumentException("ColumnValidator class can only use nullable types or a string type.");
            }

            Validator = validator ?? throw new ArgumentNullException(nameof(validator));

            PropertyName = propertyName switch
            {
                null => throw new ArgumentNullException(nameof(propertyName)),
                var s when string.IsNullOrWhiteSpace(s) => throw new ArgumentException("PropertyName cannot contain only whitespaces.", nameof(propertyName)),
                _ => propertyName
            };
        }
    }
}