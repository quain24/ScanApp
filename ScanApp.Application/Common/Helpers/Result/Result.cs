using System;
using System.Collections.Generic;

namespace ScanApp.Application.Common.Helpers.Result
{
    /// <summary>
    /// Types of valid <see cref="Result"/>
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Generic "Everything went fine" type.
        /// </summary>
        Ok,

        /// <summary>
        /// Object / instance has been created, typically used describing when database or file operations.
        /// </summary>
        Created,

        /// <summary>
        /// Object / instance has been updated, typically used describing when database or file operations.
        /// </summary>
        Updated,

        /// <summary>
        /// Data has been inserted - typically used when describing database operations.
        /// </summary>
        Inserted,

        /// <summary>
        /// Data has been replaced - typically used when describing database operations.
        /// </summary>
        Replaced,

        /// <summary>
        /// Data has been deleted successfully.
        /// </summary>
        /// <remarks>Can be used when targeted data has been already deleted and such state does not constitute an error in this particular operation.</remarks>
        Deleted,

        /// <summary>
        /// Data has not been changed.
        /// </summary>
        NotChanged
    }

    /// <summary>
    /// Types of invalid <see cref="Result"/>
    /// </summary>
    public enum ErrorType
    {
        // User related

        /// <summary>
        /// Data has not been found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Arguments given to operation were invalid, out of range, possibly forced exception to be thrown.
        /// </summary>
        WrongArguments,

        /// <summary>
        /// Generic invalid result to be used when no other can describe problem better.
        /// </summary>
        NotValid,

        /// <summary>
        /// Tried to insert data when no overwrite are allowed, filtration should report one result but found more etc.
        /// </summary>
        Duplicated,

        // Auth

        /// <summary>
        /// User should be authenticated to perform operation, but was not.
        /// </summary>
        NotAuthenticated,

        /// <summary>
        /// User should be authorized (including claims and roles) to perform operation, but was not.
        /// </summary>
        NotAuthorized,

        // Logic related

        /// <summary>
        /// Something unexpected happened - this should be used instead of generic error when caller needs to be informed, that something
        /// completely unexpected happened, like some exception that never should be thrown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Typically related to connection configuration error - database or API.
        /// </summary>
        ConfigurationError,

        /// <summary>
        /// Connection dropped out, socket failed etc.
        /// </summary>
        NetworkError,

        /// <summary>
        /// Multiple operations tried to access non thread-safe data - common in database operations.
        /// </summary>
        ConcurrencyFailure,

        /// <summary>
        /// Operation has ended due to timeout.
        /// </summary>
        Timeout,

        /// <summary>
        /// Operation has been canceled.
        /// </summary>
        Canceled,

        // Business related

        /// <summary>
        /// Operation on user account was prevented by one or more business related account rules.
        /// </summary>
        IllegalAccountOperation,

        // Database common

        /// <summary>
        /// Generic Database error to be used when no other can describe problem better.
        /// <remarks>To be used when caller is sure, that error is caused by database - otherwise use <see cref="NotValid"/></remarks>
        /// </summary>
        DatabaseError,

        /// <summary>
        /// Tried to insert a non-unique value into a database table field that require uniqueness, like primary key or unique index.
        /// </summary>
        UniqueConstraintViolation,

        /// <summary>
        /// Tried to insert <see langword="null"/> value into a non-nullable field in database.
        /// </summary>
        CannotInsertNull,

        /// <summary>
        /// Tried to insert too many characters to database field.
        /// </summary>
        MaxLengthExceeded,

        /// <summary>
        /// Tried to insert a number that is too big for targeted database field (for example a decimal which is greater then allowed db server precision.)
        /// </summary>
        NumericOverflow,

        /// <summary>
        /// Typically set when tried to delete entity that is referenced by other entity and delete behavior is set to restrict.<br/>
        /// or tried to set child entity foreign key in a parent entity that is missing from child's table or in other FK constraint problems.<br/>
        /// SQL errors for such behavior are: 1216, 1217, 1451, 1452.
        /// </summary>
        ReferenceConstraint

        ///// <summary>
        ///// Specific error code
        ///// </summary>
        //E311 = 311,
    }

    /// <summary>
    /// Represents the result of operation, be it command, query or any other operation that need a standardized return format.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets <see cref="Result"/> conclusion.
        /// </summary>
        /// <value><see langword="True"/> if the operation that gave this <see cref="Result"/> ran correctly; Otherwise, <see langword="False"/> </value>
        public bool Conclusion { get; private set; }

        /// <summary>
        /// Gets <see cref="Helpers.Result.ResultType?"/> of the operation that provided this <see cref="Result"/> instance.
        /// </summary>
        /// <remarks><see cref="Helpers.Result.ResultType?"/> is always assigned to positive outcome.</remarks>
        /// <value>Type of result that best describes completion state of operation that provided this <see cref="Result"/> instance.</value>
        public ResultType? ResultType { get; private set; }

        /// <summary>
        /// Gets description of the error.
        /// </summary>
        /// <value>Description of error if there is any, otherwise <see langword="null"/></value>
        public ErrorDescription ErrorDescription { get; private set; }

        /// <summary>
        /// Gets the output provided by operation.
        /// </summary>
        /// <value><see cref="object"/> representing <see cref="Result"/> output value, or <see langword="null"/> if none was provided.</value>
        public object Output { get; private set; }

        /// <summary>
        /// Creates new positive instance of <see cref="Result"/>. <see cref="Conclusion"/> is set to <see langword="True"/> and <see cref="ResultType"/>
        /// to <see cref="Helpers.Result.ResultType.Ok"/>.
        /// </summary>
        public Result()
        {
            Conclusion = true;
            this.ResultType = Helpers.Result.ResultType.Ok;
        }

        /// <summary>
        /// Creates new positive instance of <see cref="Result"/>. <see cref="Conclusion"/> is set to <see langword="True"/> and <see cref="ResultType"/>
        /// to given <paramref name="resultType"/>.
        /// </summary>
        /// <param name="resultType">Type of result's positive outcome</param>
        public Result(ResultType resultType)
        {
            Conclusion = true;
            this.ResultType = resultType;
        }

        /// <summary>
        /// Creates new negative instance of <see cref="Result"/>. <see cref="Conclusion"/> is set to <see langword="False"/> and <see cref="ErrorDescription"/>
        /// is created with given <paramref name="errorType"/>.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        public Result(ErrorType errorType)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription
            {
                ErrorType = errorType
            };
        }

        /// <summary>
        /// <inheritdoc cref="Result(ErrorType)"/><br/>
        /// In addition, given <paramref name="exception"/> will also be stored.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result"/>.</param>
        public Result(ErrorType errorType, Exception exception)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription
            {
                ErrorMessage = exception.Message,
                Exception = exception,
                ErrorType = errorType
            };
        }

        /// <summary>
        /// <inheritdoc cref="Result(ErrorType)"/><br/>
        /// In addition, given <paramref name="errorMessage"/> and <paramref name="exception"/> will also be stored.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="errorMessage">Message describing error</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result"/>.</param>
        public Result(ErrorType errorType, string errorMessage, string errorCode = null, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription
            {
                ErrorMessage = errorMessage,
                Exception = exception,
                ErrorType = errorType,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// <inheritdoc cref="Result(ErrorType)"/><br/>
        /// In addition, given <paramref name="errorMessages"/> and <paramref name="exception"/> will also be stored.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="errorMessages">Messages describing error, that will be stored as single <see cref="string"/> separated by "\n"</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result"/>.</param>
        public Result(ErrorType errorType, IEnumerable<string> errorMessages, string errorCode = null, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription
            {
                ErrorMessage = string.Join("\n", errorMessages),
                Exception = exception,
                ErrorType = errorType,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Sets source <see cref="Result"/> as negative and overrides its <paramref name="errorType"/>, <paramref name="errorMessage"/> and <paramref name="exception"/>
        /// with new values.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="errorMessage">Message describing error</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result"/>.</param>
        /// <returns>Instance of <see cref="Result"/> upon which this method has been run.</returns>
        public Result Set(ErrorType errorType, string errorMessage, string errorCode = null, Exception exception = null) =>
            Set(errorType, new[] { errorMessage }, errorCode, exception);

        /// <summary>
        /// Sets source <see cref="Result"/> as negative and overrides its <paramref name="errorType"/>, <paramref name="errorMessages"/> and <paramref name="exception"/>
        /// with new values.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="errorMessages">Messages describing error, that will be stored as single <see cref="string"/> separated by "\n"</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result"/>.</param>
        /// <returns>Instance of <see cref="Result"/> upon which this method has been run.</returns>
        public Result Set(ErrorType errorType, IEnumerable<string> errorMessages, string errorCode = null, Exception exception = null)
        {
            Conclusion = false;
            ErrorDescription = new ErrorDescription
            {
                ErrorMessage = string.Join("\n", errorMessages),
                Exception = exception,
                ErrorType = errorType,
                ErrorCode = errorCode
            };
            return this;
        }

        /// <summary>
        /// Sets output of this <see cref="Result"/>
        /// </summary>
        /// <param name="value">Object of type <see cref="object"/> to be stored.</param>
        /// <returns>Instance of type <see cref="Result{T}"/> upon which this method has been run.</returns>
        public Result SetOutput(object value)
        {
            Output = value;
            return this;
        }
    }

    /// <summary>
    /// Generic Result, which is able to return the desired object. So instead of return type "Customer", it would be Result &lt;Customer>
    /// </summary>
    /// <typeparam name="T">Type that this <see cref="Result{T}"/> will try to deliver in <seealso cref="Output"/> property</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Gets the output provided by operation.
        /// </summary>
        /// <value>Object of type <typeparamref name="T"/> representing <see cref="Result"/> output value, or <see langword="null"/> if none was provided.</value>
        public new T Output { get; private set; }

        /// <summary>
        /// Creates new positive instance of <see cref="Result{T}"/>. <see cref="Result{T}.Conclusion"/> is set to <see langword="True"/> and <see cref="ResultType"/>
        /// to <see cref="Helpers.Result.ResultType.Ok"/>.
        /// </summary>
        public Result() : base()
        {
        }

        /// <summary>
        /// <inheritdoc cref="Result{T}()"/>
        /// </summary>
        /// <param name="value">Output object to be stored in this <see cref="Result{T}"/>.</param>
        public Result(T value) : base() => Output = value;

        /// <summary>
        /// Creates new positive instance of <see cref="Result{T}"/>. <see cref="Result{T}.Conclusion"/> is set to <see langword="True"/> and <see cref="ResultType"/>
        /// to given <paramref name="resultType"/>.
        /// </summary>
        /// <param name="resultType">Type of result's positive outcome</param>
        public Result(ResultType resultType) : base(resultType)
        {
        }

        /// <summary>
        /// <inheritdoc cref="Result{T}(T)"/>
        /// </summary>
        /// <param name="resultType">Type of result's positive outcome</param>
        /// <param name="value">Output object to be stored in this <see cref="Result{T}"/>.</param>
        public Result(ResultType resultType, T value) : base(resultType) => Output = value;

        /// <summary>
        /// Creates new negative instance of <see cref="Result{T}"/>. <see cref="Result{T}.Conclusion"/> is set to <see langword="False"/> and <see cref="ErrorDescription"/>
        /// is created with given <paramref name="errorType"/>.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        public Result(ErrorType errorType) : base(errorType)
        {
        }

        /// <summary>
        /// <inheritdoc cref="Result{T}(ErrorType)"/><br/>
        /// In addition, given <paramref name="exception"/> will also be stored.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result{T}"/>.</param>
        public Result(ErrorType errorType, Exception exception) : base(errorType, exception)
        {
        }

        /// <summary>
        /// <inheritdoc cref="Result{T}(ErrorType)"/><br/>
        /// In addition, given <paramref name="errorMessage"/> and <paramref name="exception"/> will also be stored.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="errorMessage">Message describing error</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result{T}"/>.</param>
        public Result(ErrorType errorType, string errorMessage, string errorCode = null, Exception exception = null)
            : base(errorType, errorMessage, errorCode, exception)
        {
        }

        /// <summary>
        /// Sets source <see cref="Result{T}"/> as negative and overrides its <paramref name="errorType"/>, <paramref name="errorMessage"/> and <paramref name="exception"/>
        /// with new values.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome.</param>
        /// <param name="errorMessage">Message describing error.</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result{T}"/>.</param>
        /// <returns>Instance of <see cref="Result"/> upon which this method has been run.</returns>
        public new Result<T> Set(ErrorType errorType, string errorMessage, string errorCode = null, Exception exception = null) =>
            Set(errorType, new[] { errorMessage }, errorCode, exception);

        /// <summary>
        /// <inheritdoc cref="Result{T}(ErrorType)"/><br/>
        /// In addition, given <paramref name="errorMessages"/> and <paramref name="exception"/> will also be stored.
        /// </summary>
        /// <param name="errorType">Type of result's negative outcome</param>
        /// <param name="errorMessages">Messages describing error, that will be stored as single <see cref="string"/> separated by "\n"</param>
        /// <param name="errorCode">Additional, optional error code to precise <paramref name="errorType"/>.</param>
        /// <param name="exception">Exception to be stored in <see cref="ErrorDescription"/>, typically one that was catch inside operation that provided this <see cref="Result{T}"/>.</param>
        public new Result<T> Set(ErrorType errorType, IEnumerable<string> errorMessages, string errorCode = null, Exception exception = null)
        {
            base.Set(errorType, errorMessages, errorCode, exception);
            return this;
        }

        /// <summary>
        /// Sets output of this <see cref="Result{T}"/>
        /// </summary>
        /// <param name="value">Object of type <typeparamref name="T"/> to be stored.</param>
        /// <returns>Instance of <see cref="Result{T}"/> upon which this method has been run.</returns>
        public Result<T> SetOutput(T value)
        {
            Output = value;
            return this;
        }
    }
}