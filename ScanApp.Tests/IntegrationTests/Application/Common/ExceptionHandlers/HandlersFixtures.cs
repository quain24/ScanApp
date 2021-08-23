using EntityFramework.Exceptions.Common;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.IntegrationTests.Application.Common.ExceptionHandlers
{
    public static class ExceptionHandlerFixtures
    {
        // This list must be maintained manually
        public static readonly List<(Type Type, ErrorType ErrorType)> ManuallyProvidedHandledExceptionTypes = new()
        {
            (typeof(OperationCanceledException), ErrorType.Canceled),
            (typeof(TaskCanceledException), ErrorType.Canceled),
            (typeof(DbUpdateConcurrencyException), ErrorType.ConcurrencyFailure),
            (typeof(MaxLengthExceededException), ErrorType.MaxLengthExceeded),
            (typeof(NumericOverflowException), ErrorType.NumericOverflow),
            (typeof(ReferenceConstraintException), ErrorType.ReferenceConstraint),
            (typeof(UniqueConstraintException), ErrorType.UniqueConstraintViolation),
            (typeof(CannotInsertNullException), ErrorType.CannotInsertNull),
            (typeof(DbUpdateException), ErrorType.DatabaseError),
            (typeof(SqlException), ErrorType.DatabaseError),
            (typeof(DbExceptionFixture), ErrorType.DatabaseError)
        };

        public static List<Type> DetectedHandlerTypes { get; }
        public static List<Type> AllExcTypesExcludingHandled { get; }

        static ExceptionHandlerFixtures()
        {
            DetectedHandlerTypes = DetectedHandledTypes();
            AllExcTypesExcludingHandled = AllExceptions();
        }

        public static List<Type> DetectedHandledTypes()
        {
            // Get all handlers from all assemblies.
            var allAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x.GetName().FullName.Contains("ScanApp", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            var handlers = typeof(IRequestExceptionHandler<,,>).GetImplementingTypes(allAssemblies).ToList();
            handlers.AddRange(typeof(IRequestExceptionHandler<,>).GetImplementingTypes(allAssemblies));

            // Grab generic constraints from last generic parameter (99% exception constraint).
            var handledExceptionTypes = handlers
                .Select(x => x.GetGenericArguments().SelectMany(xx => xx.GetGenericParameterConstraints()))
                .Select(x => x.LastOrDefault(i => i.IsAssignableTo(typeof(Exception))))
                .Where(x => x is not null)
                .ToList();

            // When handled exception type cannot be detect from constraint - must be added manually.
            // Add when handled exception is derived from one that there is handler for and have no dedicated handler.
            // Add / remove when in-handler logic decides if exception should be handled or not.
            handledExceptionTypes.Add(typeof(SqlException));
            handledExceptionTypes.Add(typeof(TaskCanceledException));

            // DbException is abstract, so replace it with concrete fixture
            if (handledExceptionTypes.Remove(typeof(DbException)))
                handledExceptionTypes.Add(typeof(DbExceptionFixture));

            return handledExceptionTypes;
        }

        /// <summary>
        /// All exceptions from ScanApp and system assemblies excluding ones with detected handlers
        /// </summary>
        private static List<Type> AllExceptions()
        {
            // All common exception types excluding handled
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.Contains("ScanApp", StringComparison.OrdinalIgnoreCase) ||
                            x.FullName.Contains("system", StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsAssignableTo(typeof(Exception)) &&
                            DetectedHandledTypes().Any(type => type == x) is false)
                .ToList();

            // DbException is abstract, so replace it with concrete fixture is needed.
            // DbExceptionFixture is already found and added if necessary by above linq.
            types.Remove(typeof(DbException));
            return types.ToList();
        }

        public class HandledExceptions : TheoryData<IRequest<Result>, Type>
        {
            public HandledExceptions()
            {
                foreach (var handledType in DetectedHandlerTypes)
                {
                    Add(new Command(handledType), handledType);
                }
            }
        }

        public class NotHandledExceptions : TheoryData<IRequest<Result>>
        {
            public NotHandledExceptions()
            {
                foreach (var type in AllExcTypesExcludingHandled)
                {
                    Add(new Command(type));
                }
            }
        }

        public class DbExceptionFixture : DbException
        {
        }

        public class Command : IRequest<Result>
        {
            public Command(Type exType)
            {
                ExceptionType = exType;
            }

            public Type ExceptionType { get; }
        }

        public class GenericHandler : IRequestHandler<Command, Result>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.ExceptionType == typeof(AggregateException))
                {
                    throw new AggregateException(new Exception());
                }

                if (request.ExceptionType == typeof(DbException))
                {
                    throw new DbExceptionFixture();
                }

                dynamic exc = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(request.ExceptionType);
                throw exc;
            }
        }
    }
}