using System;

namespace ScanApp.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception to be used in case of format failure when providing / editing / etc <see cref="Entities.Location"/> name.
    /// </summary>
    [Serializable]
    public class LocationNameFormatException : Exception
    {
        /// <summary>
        /// Gets actual name value that caused this exception.
        /// </summary>
        /// <value>Value of name used in <see cref="Entities.Location"/> if set, otherwise <see lnagword="null"/>.</value>
        public string Name { get; }

        /// <summary>
        /// Creates new instance of <see cref="LocationNameFormatException"/>.
        /// </summary>
        public LocationNameFormatException()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="LocationNameFormatException"/> with given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Message that describes the error.</param>
        public LocationNameFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="LocationNameFormatException"/> with given <paramref name="message"/> and <paramref name="inner"/> exception that caused this one.
        /// </summary>
        /// <param name="message">Message that describes the error.</param>
        /// <param name="inner">Exception that caused this exception or <see lnagword="null"/> if not set.</param>
        public LocationNameFormatException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="LocationNameFormatException"/> with given <paramref name="name"/> value (actual value that was used, not a parameter name) and <paramref name="message"/>.
        /// </summary>
        /// <param name="name"><strong>Value</strong> of name that caused this exception or <see langword="null"/> if not set.</param>
        /// <param name="message">Message that describes the error.</param>
        public LocationNameFormatException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}