using System;

namespace ScanApp.Domain.Exceptions
{
    [Serializable]
    public class LocationNameFormatException : Exception
    {
        public string Name { get; }

        public LocationNameFormatException()
        {
        }

        public LocationNameFormatException(string message) : base(message)
        {
        }

        public LocationNameFormatException(string message, Exception inner) : base(message, inner)
        {
        }

        public LocationNameFormatException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}