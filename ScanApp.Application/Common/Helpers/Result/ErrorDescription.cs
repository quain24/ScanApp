using System;

namespace ScanApp.Application.Common.Helpers.Result
{
    /// <summary>
    /// Detailed error description used in <see cref="Result"/> or <see cref="Result{T}"/> objects.
    /// </summary>
    public class ErrorDescription
    {
        public ErrorType ErrorType { get; init; }

        public string ErrorMessage { get; init; }
        public string StackTrace { get; private init; }

        private readonly Exception _exception;

        public Exception Exception
        {
            get => _exception;
            init
            {
                _exception = value;
                StackTrace = _exception?.StackTrace;
            }
        }

        public Guid Guid { get; }

        public ErrorDescription()
        {
            Guid = Guid.NewGuid();
        }

        public override string ToString() => string.IsNullOrEmpty(ErrorMessage) ? ErrorType.ToString() : $"{ErrorType} - {ErrorMessage}";
    }
}