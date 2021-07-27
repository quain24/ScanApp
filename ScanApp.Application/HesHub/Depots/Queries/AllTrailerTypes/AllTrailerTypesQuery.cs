using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.Depots.Queries.AllTrailerTypes
{
    public record AllTrailerTypesQuery() : IRequest<Result<List<TrailerTypeModel>>>;

    internal class AllTrailerTypesQueryHandler : IRequestHandler<AllTrailerTypesQuery, Result<List<TrailerTypeModel>>>
    {
        private readonly IContextFactory _factory;

        public AllTrailerTypesQueryHandler(IContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<Result<List<TrailerTypeModel>>> Handle(AllTrailerTypesQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();
            var data = await ctx
                .TrailerTypes
                .AsNoTracking()
                .Select(e =>
                    new TrailerTypeModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Version = e.Version
                    })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new Result<List<TrailerTypeModel>>(data);
        }
    }
}