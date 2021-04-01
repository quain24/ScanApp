using System;

namespace ScanApp.Application.Common.Helpers.Result
{
    /// <summary>
    /// Detailed error description used in <see cref="Result"/> or <see cref="Result{T}"/> objects.
    /// </summary>
    public class ErrorDescription
    {
        public ErrorType ErrorType { get; set; }

        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }

        private Exception _exception;

        internal Exception Exc
        {
            get => _exception;
            set
            {
                _exception = value;
                StackTrace = _exception?.StackTrace;
            }
        }

        public Exception Exception => _exception;

        public Guid Guid { get; }

        public ErrorDescription()
        {
            Guid = Guid.NewGuid();
        }

        public override string ToString() => string.IsNullOrEmpty(ErrorMessage) ? ErrorType.ToString() : $"{ErrorType} - {ErrorMessage}";
    }
}