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
                return await strategy.ExecuteAsync(async token => await EditingStrategy(token, request), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<Version>(ErrorType.Canceled, ex);
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

        private async Task<Result<Version>> EditingStrategy(CancellationToken token, EditDepotCommand request)
        {
            await using var ctx = _factory.CreateDbContext();
            await using var dbContextTransaction = await ctx.Database.BeginTransactionAsync(token).ConfigureAwait(false);
            var (originalModel, editedModel) = request;

            var orgChildren = CreateChildren(originalModel);
            var newChildren = CreateChildren(editedModel);

            var originalDepot = MapFrom(originalModel);
            originalDepot.DefaultGate = orgChildren.Gate;
            originalDepot.DefaultTrailer = orgChildren.Trailer;
            var editedDepot = MapFrom(editedModel);
            editedDepot.DefaultGate = newChildren.Gate;
            editedDepot.DefaultTrailer = newChildren.Trailer;

            if (originalDepot.Id != editedDepot.Id)
            {
                ctx.Remove(originalDepot);
                await ctx.SaveChangesAsync(token).ConfigureAwait(false);
                // Must detach or 'Add' will throw
                if (originalDepot.DefaultGate is not null)
                    ctx.Entry(originalDepot.DefaultGate).State = EntityState.Detached;
                if (originalDepot.DefaultTrailer is not null)
                    ctx.Entry(originalDepot.DefaultTrailer).State = EntityState.Detached;
                ctx.Add(editedDepot);
                // Do not want to re-save existing entities, just update navigation in parent.
                if (editedDepot.DefaultGate is not null)
                    ctx.Entry(editedDepot.DefaultGate).State = EntityState.Unchanged;
                if (editedDepot.DefaultTrailer is not null)
                    ctx.Entry(editedDepot.DefaultTrailer).State = EntityState.Unchanged;
            }
            else
            {
                ctx.Depots.Attach(originalDepot);
                ctx.Entry(originalDepot).CurrentValues.SetValues(editedDepot);
                if (originalDepot.Address != editedDepot.Address)
                    originalDepot.ChangeAddress(editedDepot.Address);
                if (originalDepot.DefaultGate?.Id != editedDepot.DefaultGate?.Id)
                {
                    originalDepot.DefaultGate = editedDepot.DefaultGate;
                    if (originalDepot.DefaultGate is not null)
                        ctx.Entry(originalDepot.DefaultGate).State = EntityState.Unchanged;
                }
                if (originalDepot.DefaultTrailer?.Id != editedDepot.DefaultTrailer?.Id)
                {
                    originalDepot.DefaultTrailer = editedDepot.DefaultTrailer;
                    if (originalDepot.DefaultTrailer is not null)
                        ctx.Entry(originalDepot.DefaultTrailer).State = EntityState.Unchanged;
                }
            }

            token.ThrowIfCancellationRequested();
            var saved = await ctx.SaveChangesAsync(token).ConfigureAwait(false);
            await dbContextTransaction.CommitAsync(token).ConfigureAwait(false);

            return saved >= 1
                ? new Result<Version>(ResultType.Updated, editedDepot.Id != originalDepot.Id ? editedDepot.Version : originalDepot.Version)
                : new Result<Version>(ResultType.NotChanged, originalDepot.Version);
        }

        private static (Gate Gate, TrailerType Trailer) CreateChildren(DepotModel model)
        {
            var gate = model.DefaultGate is null
                ? null
                : new Gate(-100, 0)
                {
                    Version = model.DefaultGate.Version,
                    Id = model.DefaultGate.Id
                };
            var trailer = model.DefaultTrailer is null
                ? null
                : new TrailerType("valid")
                {
                    Version = model.DefaultTrailer.Version,
                    Id = model.DefaultTrailer.Id
                };

            return (gate, trailer);
        }

        private static Depot MapFrom(DepotModel model)
        {
            var depot = new Depot(model.Id, model.Name, model.PhoneNumber, model.Email,
                Address.Create(model.StreetName, model.ZipCode, model.City, model.Country));
            depot.ChangeVersion(model.Version);
            depot.ChangeDistanceToHub(model.DistanceToDepot);

            return depot;
        }
    }
}