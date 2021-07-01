using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Hubs.Commands.EditHub
{
    public record EditHubCommand(HesHubModel OriginalModel, HesHubModel EditedModel) : IRequest<Result<Version>>;

    internal class EditHubCommandHandler : IRequestHandler<EditHubCommand, Result<Version>>
    {
        private readonly IContextFactory _factory;

        public EditHubCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<Version>> Handle(EditHubCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _factory.CreateDbContext();
                var strategy = ctx.Database.CreateExecutionStrategy();

                return await strategy.ExecuteAsync(async token =>
                {
                    using (var dbContextTransaction = await ctx.Database.BeginTransactionAsync(token))
                    {
                        var (originalModel, editedModel) = request;

                        var originalDepot = new HesDepot(originalModel.Id, originalModel.Name,
                            Address.Create(originalModel.StreetName, originalModel.StreetNumber, originalModel.ZipCode, originalModel.City, originalModel.Country),
                            originalModel.PhonePrefix, originalModel.PhoneNumber, originalModel.Email);
                        originalDepot.ChangeVersion(originalModel.Version);
                        var editedDepot = new HesDepot(editedModel.Id, editedModel.Name,
                            Address.Create(editedModel.StreetName, editedModel.StreetNumber, editedModel.ZipCode, editedModel.City, editedModel.Country),
                            editedModel.PhonePrefix, editedModel.PhoneNumber, editedModel.Email);
                        editedDepot.ChangeVersion(editedModel.Version);

                        if (originalDepot.Id != editedDepot.Id)
                        {
                            ctx.Remove(originalDepot);
                            ctx.SaveChanges();
                            ctx.Add(editedDepot);
                        }
                        else
                        {
                            ctx.HesDepots.Attach(originalDepot);
                            ctx.Entry(originalDepot).CurrentValues.SetValues(editedDepot);
                            if (originalDepot.Address != editedDepot.Address)
                                originalDepot.ChangeAddress(editedDepot.Address);
                        }

                        token.ThrowIfCancellationRequested();
                        var saved = await ctx.SaveChangesAsync(token).ConfigureAwait(false);
                        await dbContextTransaction.CommitAsync(token);

                        return saved == 1
                            ? new Result<Version>(ResultType.Updated, editedDepot.Id != originalDepot.Id ? editedDepot.Version : originalDepot.Version)
                            : new Result<Version>(ErrorType.Unknown);
                    }
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