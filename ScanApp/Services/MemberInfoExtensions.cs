using System;
using System.Reflection;

namespace ScanApp.Services
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Extracts underlying <see cref="Type"/> from given <paramref name="info"/>.<br/>
        /// Type can be extracted only if source <see cref="MemberInfo"/> is either a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="info">Object to have its underlying type extracted</param>
        /// <returns>Type of object from which this <see cref="MemberInfo"/> was extracted.</returns>
        /// <exception cref="ArgumentException">Given <paramref name="info"/> was not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        public static Type GetUnderlyingType(this MemberInfo info)
        {
            return info?.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)info).FieldType,
                MemberTypes.Property => ((PropertyInfo)info).PropertyType,
                _ => throw new ArgumentException("Underlying type is returned only for properties and fields.")
            };
        }
    }
}