using System;

namespace ScanApp.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Checks if given <paramref name="values"/> are defined in a corresponding <c>[flag] enum</c>.<br/>
        /// </summary>
        /// <param name="values">One or more values of <c>[flag] enumeration</c></param>
        /// <returns><see langword="True"/> if given <paramref name="values"/> represent a valid flag combination, otherwise <see langword="false"/>.</returns>
        public static bool IsDefinedFlag(this Enum values)
        {
            var firstDigit = values.ToString()[0];
            return !char.IsDigit(firstDigit) && firstDigit != '-';
        }
    }
}