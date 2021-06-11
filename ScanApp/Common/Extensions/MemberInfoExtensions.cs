using System;
using System.Reflection;

namespace ScanApp.Common.Extensions
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Extracts underlying <see cref="Type"/> from given <paramref name="info"/>.<br/>
        /// Type can be extracted only if source <see cref="MemberInfo"/> is either a <see cref="FieldInfo"/>, <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="info">Object to have its underlying type extracted</param>
        /// <returns>Type of object from which this <see cref="MemberInfo"/> was extracted.</returns>
        /// <exception cref="ArgumentException">
        /// Given <paramref name="info"/> was not <see cref="FieldInfo"/>,
        /// <see cref="PropertyInfo"/> or <see cref="MethodInfo"/> or <see cref="MethodInfo"/> has return parameter of <see langword="void"/>.
        /// </exception>
        public static Type GetUnderlyingType(this MemberInfo info)
        {
            return info?.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)info).FieldType,
                MemberTypes.Property => ((PropertyInfo)info).PropertyType,
                MemberTypes.Method when ((MethodInfo)info).ReturnType != typeof(void) => ((MethodInfo)info).ReturnType,
                _ => throw new ArgumentException("Underlying type can be returned for properties, fields or a return type for non-void method calls...")
            };
        }
    }
}