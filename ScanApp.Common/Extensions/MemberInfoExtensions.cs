﻿using System;
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

        /// <summary>
        /// Checks if given <paramref name="info"/> is from object that can be written to.<br/>
        /// Works when <paramref name="info"/> is either a <see cref="FieldInfo"/>, <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="info">Object to be checked.</param>
        /// <returns><see langord="true"/> if <paramref name="info"/> is taken from something writable, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Given <paramref name="info"/> was not <see cref="FieldInfo"/>,
        /// <see cref="PropertyInfo"/> or <see cref="MethodInfo"/> or <see cref="MethodInfo"/> has return parameter of <see langword="void"/>.
        /// </exception>
        public static bool IsWritable(this MemberInfo info)
        {
            return info?.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)info).IsInitOnly is false,
                MemberTypes.Property => ((PropertyInfo)info).CanWrite,
                MemberTypes.Method => false,
                _ => throw new ArgumentException("Underlying type can be returned for properties, fields or a return type for non-void method calls...")
            };
        }
    }
}
