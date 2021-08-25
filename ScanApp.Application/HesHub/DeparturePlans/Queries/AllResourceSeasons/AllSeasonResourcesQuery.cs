using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllResourceSeasons
{
    public record AllSeasonResourcesQuery : IRequest<Result<IEnumerable<SeasonResourceModel>>>;

    internal class AllSeasonResourcesQueryHandler : IRequestHandler<AllSeasonResourcesQuery, Result<IEnumerable<SeasonResourceModel>>>
    {
        private readonly IContextFactory _factory;

        public AllSeasonResourcesQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<IEnumerable<SeasonResourceModel>>> Handle(AllSeasonResourcesQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var data = await ctx.Seasons.Select(x =>
                    new SeasonResourceModel
                    {
                        Name = x.Name,
                        Version = x.Version
                    })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Result<IEnumerable<SeasonResourceModel>>(data);
        }
    }
}