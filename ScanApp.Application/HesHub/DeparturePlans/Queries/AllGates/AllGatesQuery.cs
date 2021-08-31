using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllGates
{
    public record AllGatesQuery : IRequest<Result<IEnumerable<GateModel>>>;

    internal class AllGatesQueryHandler : IRequestHandler<AllGatesQuery, Result<IEnumerable<GateModel>>>
    {
        private readonly IContextFactory _factory;

        public AllGatesQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<IEnumerable<GateModel>>> Handle(AllGatesQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var data = await ctx.Gates
                .AsNoTracking()
                .Select(x => new GateModel
                {
                    Id = x.Id,
                    Name = x.Number.ToString(),
                    Version = x.Version,
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Result<IEnumerable<GateModel>>(data);
        }
    }
}