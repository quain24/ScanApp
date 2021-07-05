using System;
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

namespace ScanApp.Application.HesHub.Depots.Commands.CreateNewHub
{
    public record CreateNewHubCommand(DepotModel Model) : IRequest<Result<Version>>;

    internal class CreateNewHubCommandHandler : IRequestHandler<CreateNewHubCommand, Result<Version>>
    {
        private readonly IContextFactory _factory;

        public CreateNewHubCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<Version>> Handle(CreateNewHubCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _factory.CreateDbContext();

                var model = request.Model;
                var depot = new Depot(model.Id, model.Name,
                    Address.Create(model.StreetName, model.StreetNumber, model.ZipCode, model.City, model.Country),
                    model.PhonePrefix, model.PhoneNumber, model.Email);

                await ctx.Depots.AddAsync(depot, cancellationToken).ConfigureAwait(false);
                var saved = await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return saved == 1 ? new Result<Version>(ResultType.Created).SetOutput(depot.Version) : new Result<Version>(ErrorType.Unknown);
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