﻿using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots.Commands.EditDepot
{
    public record EditDepotCommand(DepotModel OriginalModel, DepotModel EditedModel) : IRequest<Result<Version>>;

    internal class EditDepotCommandHandler : IRequestHandler<EditDepotCommand, Result<Version>>
    {
        private readonly IContextFactory _factory;

        public EditDepotCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<Version>> Handle(EditDepotCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var context = _factory.CreateDbContext();
                var strategy = context.Database.CreateExecutionStrategy();

                return await strategy.ExecuteAsync(async token =>
                {
                    await using var ctx = _factory.CreateDbContext();
                    await using var dbContextTransaction = await ctx.Database.BeginTransactionAsync(token).ConfigureAwait(false);
                    var (originalModel, editedModel) = request;

                    var originalDepot = new Depot(originalModel.Id, originalModel.Name,originalModel.PhoneNumber, originalModel.Email,
                        Address.Create(originalModel.StreetName, originalModel.ZipCode, originalModel.City, originalModel.Country));
                    originalDepot.ChangeVersion(originalModel.Version);
                    var editedDepot = new Depot(editedModel.Id, editedModel.Name, editedModel.PhoneNumber, editedModel.Email,
                        Address.Create(editedModel.StreetName, editedModel.ZipCode, editedModel.City, editedModel.Country));
                    editedDepot.ChangeVersion(editedModel.Version);

                    if (originalDepot.Id != editedDepot.Id)
                    {
                        ctx.Remove(originalDepot);
                        ctx.SaveChanges();
                        ctx.Add(editedDepot);
                    }
                    else
                    {
                        ctx.Depots.Attach(originalDepot);
                        ctx.Entry(originalDepot).CurrentValues.SetValues(editedDepot);
                        if (originalDepot.Address != editedDepot.Address)
                            originalDepot.ChangeAddress(editedDepot.Address);
                    }

                    token.ThrowIfCancellationRequested();
                    var saved = await ctx.SaveChangesAsync(token).ConfigureAwait(false);
                    await dbContextTransaction.CommitAsync(token).ConfigureAwait(false);

                    return saved == 1
                        ? new Result<Version>(ResultType.Updated, editedDepot.Id != originalDepot.Id ? editedDepot.Version : originalDepot.Version)
                        : new Result<Version>(ResultType.NotChanged, originalDepot.Version);
                }, cancellationToken);
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