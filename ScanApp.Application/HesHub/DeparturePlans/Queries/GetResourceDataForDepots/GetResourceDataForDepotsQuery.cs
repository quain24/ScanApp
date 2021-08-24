using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.GetResourceDataForDepots
{
    public record GetResourceDataForDepotsQuery : IRequest<Result<IEnumerable<DepotResourceModel>>>;

    internal class GetResourceDataForDepotsQueryHandler : IRequestHandler<GetResourceDataForDepotsQuery, Result<IEnumerable<DepotResourceModel>>>
    {
        private readonly IContextFactory _factory;

        public GetResourceDataForDepotsQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<IEnumerable<DepotResourceModel>>> Handle(GetResourceDataForDepotsQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var data = await ctx.Depots.Select(x =>
                new DepotResourceModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Version = x.Version
                }
            ).ToListAsync(cancellationToken);

            return new Result<IEnumerable<DepotResourceModel>>(data);
        }
    }
}