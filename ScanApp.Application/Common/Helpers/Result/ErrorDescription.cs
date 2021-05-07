﻿using System;

namespace ScanApp.Application.Common.Helpers.Result
{
    /// <summary>
    /// Represents detailed error description used in <see cref="Result"/> or <see cref="Result{T}"/> objects.
    /// </summary>
    public class ErrorDescription
    {
        /// <summary>
        /// Gets error code / type.
        /// </summary>
        /// <value>Code / type of error stored in this object</value>
        public ErrorType ErrorType { get; init; }

        /// <summary>
        /// Gets error message.
        /// </summary>
        /// <value>Error message stored in this object or <see langword="null"/> if no message is stored.</value>
        public string ErrorMessage { get; init; }

        /// <summary>
        /// Gets stack trace of <see cref="System.Exception"/> stored in this object, if any.
        /// </summary>
        /// <value> Stack trace of <see cref="System.Exception"/> stored in this object if any, otherwise <see langword="null"/>.</value>
        public string StackTrace { get; private init; }

        private readonly Exception _exception;

        /// <summary>
        /// Exception that is stored in this object
        /// </summary>
        /// <remarks>Setting exception also automatically sets the <see cref="StackTrace"/> property</remarks>
        /// <value><see cref="System.Exception"/> stored in this object, or <see langword="null"/> otherwise</value>
        public Exception Exception
        {
            get => _exception;
            init
            {
                _exception = value;
                StackTrace = _exception?.StackTrace;
            }
        }

        /// <summary>
        /// Gets the unique identifier of this <see cref="ErrorDescription"/> instance
        /// </summary>
        /// <value>Unique <see cref="System.Guid"/> identifier</value>
        public Guid Guid { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ErrorDescription"/>
        /// </summary>
        public ErrorDescription()
        {
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// <see cref="string"/> representation of this instance, containing its <see cref="ErrorType"/> and <see cref="ErrorMessage"/>.
        /// </summary>
        /// <remarks> Format of the string is <c>ErrorMessage - ErrorType</c> if both are present, or just <c>ErrorCode</c> if there is no message.</remarks>
        /// <returns><para><see cref="string"/> containing <see cref="ErrorType"/> and <see cref="ErrorMessage"/> if both are present.</para>
        /// <para><see cref="string"/> containing just <see cref="ErrorType"/> if there is no error message.</para>
        /// </returns>
        public override string ToString() => string.IsNullOrEmpty(ErrorMessage) ? ErrorType.ToString() : $"{ErrorType} - {ErrorMessage}";
    }
}