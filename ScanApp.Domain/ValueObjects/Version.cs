using ScanApp.Domain.Common;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.ValueObjects
{
    /// <summary>
    /// Represents an object encapsulating raw <c>concurrency stamp</c> from application's data store.
    /// </summary>
    public sealed class Version : ValueObject
    {
        /// <summary>
        /// Creates new instance of <see cref="Version"/> with given <paramref name="stamp"/> value.
        /// </summary>
        /// <param name="stamp">Concurrency stamp.</param>
        /// <returns>Instance of <see cref="Version"/> encapsulating given <paramref name="stamp"/>.</returns>
        /// <exception cref="ArgumentNullException">Tried to create <see cref="Version"/> instance with <see langword="null"/> value.</exception>
        /// <exception cref="FormatException">Tried to create <see cref="Version"/> instance using only white-spaces as <paramref name="stamp"/>.</exception>
        public static Version Create(string stamp) => new(stamp);

        /// <summary>
        /// Creates new <strong>empty</strong> instance of <see cref="Version"/>.<br/>
        /// Such instance does not have a <see cref="Value"/> set and it's <see cref="IsEmpty"/> returns <see langword="True"/>.
        /// </summary>
        /// <returns>Instance of empty <see cref="Version"/>.</returns>
        public static Version Empty() => new();

        private Version(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), $"Do not create {nameof(Version)} with NULL value. To create empty {nameof(Version)} use {nameof(Version)}.{nameof(Empty)}");

            if (string.IsNullOrWhiteSpace(value))
                throw new FormatException($"{nameof(value)} cannot be empty and must contain a value other than just whitespaces. For empty version use {nameof(Version)}.{nameof(Empty)}");

            Value = value;
        }

        private Version()
        {
        }

        /// <summary>
        /// Gets raw <see cref="string"/> value of concurrency stamp that this instance wraps.
        /// </summary>
        /// <value>Value of this instance concurrency stamp or <see langword="null"/> if this <see cref="Version"/> is empty.</value>
        public string Value { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of <see cref="Version"/> is empty (was created by using <see cref="Empty"/>) and therefore has no value.
        /// </summary>
        public bool IsEmpty => Value is null;

        /// <summary>
        /// Returns <see cref="string"/> representation of this <see cref="Version"/> instance, which is <see cref="Value"/>.
        /// </summary>
        /// <returns><see cref="Value"/> if there is one set, otherwise <see langword="null"/>.</returns>
        public override string ToString() => Value;

        public static implicit operator string(Version stamp) => stamp.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}