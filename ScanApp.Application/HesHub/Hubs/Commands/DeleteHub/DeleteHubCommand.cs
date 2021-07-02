﻿using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using Version = ScanApp.Domain.ValueObjects.Version;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace ScanApp.Application.HesHub.Hubs.Commands.DeleteHub
{
    public record DeleteHubCommand(int Id, Version Version) : IRequest<Result>;

    internal class DeleteHubCommandHandler : IRequestHandler<DeleteHubCommand, Result>
    {
        private readonly IContextFactory _factory;

        public DeleteHubCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result> Handle(DeleteHubCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _factory.CreateDbContext();
                var depot = new HesDepot(request.Id, "name",
                    Address.Create("name", null, "name", "name", "name"),
                    "0", "0", "e@m.c");
                depot.ChangeVersion(request.Version);

                ctx.Remove(depot);
                var removed = await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return removed > 0 ? new Result(ResultType.Deleted) : new Result(ErrorType.NotFound);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<Version>(ErrorType.Cancelled, ex);
            }
            catch (DbUpdateException ex)
            {
                return ex is DbUpdateConcurrencyException
                    ? new Result<Version>(ErrorType.ConcurrencyFailure, ex.InnerException?.Message ?? ex.Message, ex)
                    : new Result<Version>(ErrorType.DatabaseError, ex.InnerException?.Message ?? ex.Message, ex);
            }
            catch (SqlException ex)
            {
                return new Result<Version>(ErrorType.DatabaseError, ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
    }
}