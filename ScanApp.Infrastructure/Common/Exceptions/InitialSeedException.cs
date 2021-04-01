using System;

namespace ScanApp.Infrastructure.Common.Exceptions
{
    [Serializable]
    public class InitialSeedException : Exception
    {
        public string SeedPart { get; }

        public InitialSeedException() : base()
        {
        }

        public InitialSeedException(string message) : base(message)
        {
        }

        public InitialSeedException(string seedPart, string message) : base(message)
        {
            SeedPart = seedPart;
        }

        public InitialSeedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InitialSeedException(string seedPart, string message, Exception innerException) : base(message, innerException)
        {
            SeedPart = seedPart;
        }
    }
}