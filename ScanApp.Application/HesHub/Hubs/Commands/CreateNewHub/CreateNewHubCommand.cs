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

namespace ScanApp.Application.HesHub.Hubs.Commands.CreateNewHub
{
    public record CreateNewHubCommand(HesHubModel Model) : IRequest<Result>;

    internal class CreateNewHubCommandHandler : IRequestHandler<CreateNewHubCommand, Result>
    {
        private readonly IContextFactory _factory;

        public CreateNewHubCommandHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result> Handle(CreateNewHubCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _factory.CreateDbContext();

                var model = request.Model;
                var depot = new HesDepot(model.Id, model.Name,
                    Address.Create(model.StreetName, model.StreetNumber, model.ZipCode, model.City, model.Country),
                    model.PhonePrefix, model.PhoneNumber, model.Email);

                await ctx.HesDepots.AddAsync(depot, cancellationToken).ConfigureAwait(false);
                var saved = await ctx.SaveChangesAsync(cancellationToken);

                return saved == 1 ? new Result(ResultType.Created) : new Result(ErrorType.Unknown);
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