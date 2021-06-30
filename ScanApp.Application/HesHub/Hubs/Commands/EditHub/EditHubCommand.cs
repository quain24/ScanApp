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

namespace ScanApp.Application.HesHub.Hubs.Commands.EditHub
{
    public record EditHubCommand(HesHubModel OriginalModel, HesHubModel EditedModel) : IRequest<Result>;

    internal class EditHubCommandHandler : IRequestHandler<EditHubCommand, Result>
    {
        private readonly IContextFactory _factory;

        public EditHubCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result> Handle(EditHubCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _factory.CreateDbContext();

                var (originalModel, editedModel) = request;

                var originalDepot = new HesDepot(originalModel.Id, originalModel.Name,
                    Address.Create(originalModel.StreetName, originalModel.StreetNumber, originalModel.ZipCode, originalModel.City, originalModel.Country),
                    originalModel.PhonePrefix, originalModel.PhoneNumber, originalModel.Email);
                var editedDepot = new HesDepot(editedModel.Id, editedModel.Name,
                    Address.Create(editedModel.StreetName, editedModel.StreetNumber, editedModel.ZipCode, editedModel.City, editedModel.Country),
                    editedModel.PhonePrefix, editedModel.PhoneNumber, editedModel.Email);

                cancellationToken.ThrowIfCancellationRequested();
                ctx.HesDepots.Attach(originalDepot);
                ctx.HesDepots.Update(editedDepot);
                var saved = await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return saved == 1 ? new Result(ResultType.Updated) : new Result(ErrorType.Unknown);
            }
            catch (OperationCanceledException ex)
            {
                return new Result(ErrorType.Cancelled, ex);
            }
            catch (DbUpdateException ex)
            {
                return ex is DbUpdateConcurrencyException
                    ? new Result(ErrorType.ConcurrencyFailure, ex.InnerException?.Message ?? ex.Message, ex)
                    : new Result(ErrorType.DatabaseError, ex.InnerException?.Message ?? ex.Message, ex);
            }
            catch (SqlException ex)
            {
                return new Result(ErrorType.DatabaseError, ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
    }
}