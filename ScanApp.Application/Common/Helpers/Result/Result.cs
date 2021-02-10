﻿using System;
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
        Deleted
    }

    public enum ErrorType
    {
        // User related

        NotFound,
        WrongArguments,
        NotValid,

        // Auth

        NoAuthentication,
        NotAuthorized,

        // Logic related

        Unknown,
        ConfigurationError,
        NetworkError,

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
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorType = errorType;
        }

        public Result(ErrorType errorType, Exception exception)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = exception.Message;
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result(ErrorType errorType, string errorMessage, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = errorMessage;
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result(ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = string.Join("\n", errorMessages);
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public void Set(ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = string.Join("\n", errorMessages);
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
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
        public T Output { get; private set; }

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

        public Result(ErrorDescription errorDescription) : base(errorDescription.ErrorType, errorDescription.ErrorMessage, errorDescription.Exception)
        {
        }

        public Result<T> AddMethodInfo(params string[] infos)
        {
            // Prepend error message with additional info
            base.ErrorDescription.ErrorMessage = $"{typeof(T).Name} {string.Join(", ", infos)}, {ErrorDescription.ErrorMessage}";
            return this;
        }

        public Result<T> SetOutput(T value)
        {
            var t = new Result<string>("aaa");
            Output = value;
            return this;
        }
    }
}