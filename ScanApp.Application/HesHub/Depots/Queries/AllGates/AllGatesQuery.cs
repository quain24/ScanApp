using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.Depots.Queries.AllGates
{
    public record AllGatesQuery() : IRequest<Result<List<GateModel>>>;

    internal class AllGatesQueryHandler : IRequestHandler<AllGatesQuery, Result<List<GateModel>>>
    {
        private readonly IContextFactory _factory;

        public AllGatesQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<List<GateModel>>> Handle(AllGatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _factory.CreateDbContext();
                var data = await ctx
                    .Gates
                    .AsNoTracking()
                    .Select(e =>
                        new GateModel
                        {
                            Id = e.Id,
                            Number = e.Number,
                            Version = e.Version
                        })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                return new Result<List<GateModel>>(data);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<GateModel>>(ErrorType.Cancelled, ex);
            }
            catch (SqlException ex)
            {
                return new Result<List<GateModel>>(ErrorType.DatabaseError, ex?.InnerException?.Message, ex);
            }
        }
    }
}