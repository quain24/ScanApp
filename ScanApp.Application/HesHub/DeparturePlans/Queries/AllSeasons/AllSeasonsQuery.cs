using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllSeasons
{
    public record AllSeasonsQuery : IRequest<Result<IEnumerable<SeasonModel>>>;

    internal class AllSeasonsQueryHandler : IRequestHandler<AllSeasonsQuery, Result<IEnumerable<SeasonModel>>>
    {
        private readonly IContextFactory _factory;

        public AllSeasonsQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<IEnumerable<SeasonModel>>> Handle(AllSeasonsQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var data = await ctx.Seasons.Select(x =>
                    new SeasonModel
                    {
                        Name = x.Name,
                        Start = x.Start,
                        End = x.End,
                        Version = x.Version
                    })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Result<IEnumerable<SeasonModel>>(data);
        }
    }
}