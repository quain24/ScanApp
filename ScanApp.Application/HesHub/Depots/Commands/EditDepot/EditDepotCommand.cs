using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                    // Grab original child Id's and check if original depot still exists in one go.
                    var originalChildren = ctx
                        .Depots
                        .AsNoTracking()
                        .Where(x => x.Id.Equals(originalModel.Id))
                        .Select(x => new
                        {
                            GateId = EF.Property<int?>(x, "DefaultGateId"),
                            TrailerId = EF.Property<int?>(x, "DefaultTrailerId")
                        }).SingleOrDefault();

                    if (originalChildren is null)
                        return new Result<Version>(ErrorType.NotFound);

                    var originalDepot = MapFrom(originalModel);
                    var editedDepot = MapFrom(editedModel);

                    if (originalDepot.Id != editedDepot.Id)
                    {
                        ctx.Remove(originalDepot);
                        ctx.SaveChanges();
                        ctx.Add(editedDepot);
                        ctx.Entry(editedDepot).Property("DefaultGateId").CurrentValue = editedModel.DefaultGate?.Id;
                        ctx.Entry(editedDepot).Property("DefaultTrailerId").CurrentValue = editedModel.DefaultTrailer?.Id;
                    }
                    else
                    {
                        ctx.Depots.Attach(originalDepot);
                        ctx.Entry(originalDepot).CurrentValues.SetValues(editedDepot);
                        if (originalDepot.Address != editedDepot.Address)
                            originalDepot.ChangeAddress(editedDepot.Address);

                        // Using shadow properties - do not need version data and if child was deleted
                        // or out of range - SQL exception is thrown.
                        if (editedModel.DefaultGate?.Id != originalChildren.GateId)
                            ctx.Entry(originalDepot).Navigation("DefaultGate").IsModified = true;

                        if (editedModel.DefaultTrailer?.Id != originalChildren.TrailerId)
                            ctx.Entry(originalDepot).Navigation("DefaultTrailer").IsModified = true;
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

        private Depot MapFrom(DepotModel model)
        {
            var depot = new Depot(model.Id, model.Name, model.PhoneNumber, model.Email,
                Address.Create(model.StreetName, model.ZipCode, model.City, model.Country));
            depot.ChangeVersion(model.Version);
            depot.ChangeDistanceToHub(model.DistanceToDepot);

            return depot;
        }
    }
}