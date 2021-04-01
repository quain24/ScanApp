using System;
using System.Collections.Generic;

namespace ScanApp.Application.Common.Helpers.Result
{
    public enum ResultType
    {
        Ok,
        Created,
        Updated,
        Inserted,
        Replaced,
        Deleted,
        NotChanged
    }

    public enum ErrorType
    {
        // User related

        NotFound,
        WrongArguments,
        NotValid,
        Duplicated,

        // Auth

        NoAuthentication,
        NotAuthorized,

        // Logic related

        Unknown,
        ConfigurationError,
        NetworkError,
        ConcurrencyFailure,
        Timeout

        ///// <summary>
        ///// Specific error code
        ///// </summary>
        //E311 = 311,
    }

    /// <summary>
    /// The Result itself.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Is set to true if the method ran correcty.
        /// </summary>
        public bool Conclusion { get; private set; }

        /// <summary>
        /// ResultType of the method.
        /// </summary>
        public ResultType? ResultType { get; private set; }

        /// <summary>
        /// Description of the error. Can be null.
        /// </summary>
        public ErrorDescription ErrorDescription { get; private set; }

        public object Output { get; private set; }

        /// <summary>
        /// Positive result. Conclusion is true and result type is Ok.
        /// </summary>
        public Result()
        {
            Conclusion = true;
            this.ResultType = Helpers.Result.ResultType.Ok;
        }

        /// <summary>
        /// Positive result. Conclusion is true and result type is given as argument.
        /// </summary>
        /// <param name="resultType"></param>
        public Result(ResultType resultType)
        {
            Conclusion = true;
            this.ResultType = resultType;
        }

        /// <summary>
        /// Negative result. Conclusion is false.
        /// </summary>
        /// <param name="errorType"></param>
        public Result(ErrorType errorType)
        {
            Conclusion = false;
            ErrorDescription = new();
            ErrorDescription.ErrorType = errorType;
        }

        public Result(ErrorType errorType, Exception exception)
        {
            Conclusion = false;
            ErrorDescription = new();
            ErrorDescription.ErrorMessage = exception.Message;
            ErrorDescription.Exc = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result(ErrorType errorType, string errorMessage, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new();
            ErrorDescription.ErrorMessage = errorMessage;
            ErrorDescription.Exc = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result(ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new();
            ErrorDescription.ErrorMessage = string.Join("\n", errorMessages);
            ErrorDescription.Exc = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result Set(ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = string.Join("\n", errorMessages);
            ErrorDescription.Exc = exception;
            ErrorDescription.ErrorType = errorType;
            return this;
        }

        public Result SetOutput(object value)
        {
            Output = value;
            return this;
        }
    }

    /// <summary>
    /// Generic Result, which is able to return the desired object. So instead of return typ "Customer", it would be Result &lt;Customer>
    /// </summary>
    /// <typeparam name="T">Type that this <see cref="Result{T}"/> will try to deliver in <seealso cref="Output"/> property</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// The desired output.
        /// </summary>
        public new T Output { get; private set; }

        public Result() : base()
        {
        }

        public Result(T value) : base() => Output = value;

        public Result(ResultType resultType) : base(resultType)
        {
        }

        public Result(ResultType resultType, T value) : base(resultType) => Output = value;

        public Result(ErrorType errorType) : base(errorType)
        {
        }

        public Result(ErrorType errorType, Exception exception) : base(errorType, exception)
        {
        }

        public Result(ErrorType errorType, string errorMessage, Exception exception = null) : base(errorType, errorMessage, exception)
        {
        }

        public Result(ErrorDescription errorDescription) : base(errorDescription.ErrorType, errorDescription.ErrorMessage, errorDescription.Exc)
        {
        }

        public Result<T> AddMethodInfo(params string[] infos)
        {
            // Prepend error message with additional info
            base.ErrorDescription.ErrorMessage = $"{typeof(T).Name} {string.Join(", ", infos)}, {ErrorDescription.ErrorMessage}";
            return this;
        }

        public new Result<T> Set(ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null)
        {
            base.Set(errorType, errorMessages, exception);
            return this;
        }

        public Result<T> SetOutput(T value)
        {
            Output = value;
            return this;
        }
    }
}