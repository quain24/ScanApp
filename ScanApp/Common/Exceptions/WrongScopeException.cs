using System;

namespace ScanApp.Common.Exceptions
{
    public class WrongScopeException : Exception
    {
        public WrongScopeException() : base()
        {
        }

        public WrongScopeException(string message) : base(message)
        {
        }

        public WrongScopeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}