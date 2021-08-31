using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllTrailerTypes
{
    public record AllTrailerTypesQuery : IRequest<Result<IEnumerable<TrailerModel>>>;

    internal class AllTrailerTypesQueryHandler : IRequestHandler<AllTrailerTypesQuery, Result<IEnumerable<TrailerModel>>>
    {
        private readonly IContextFactory _factory;

        public AllTrailerTypesQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<IEnumerable<TrailerModel>>> Handle(AllTrailerTypesQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var data = await ctx.TrailerTypes
                .AsNoTracking()
                .Select(x => new TrailerModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Version = x.Version
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Result<IEnumerable<TrailerModel>>(data);
        }
    }
}