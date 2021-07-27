using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.Depots.Queries.AllDepots
{
    public record AllDepotsQuery : IRequest<Result<List<DepotModel>>>;

    internal class AllDepotsQueryHandler : IRequestHandler<AllDepotsQuery, Result<List<DepotModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public AllDepotsQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<DepotModel>>> Handle(AllDepotsQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            var result = await ctx
                .Depots
                .AsNoTracking()
                .Include(e => e.DefaultGate)
                .Include(e => e.DefaultTrailer)
                .Select(h => new DepotModel()
                {
                    City = h.Address.City,
                    Country = h.Address.Country,
                    Email = h.Email,
                    Id = h.Id,
                    StreetName = h.Address.StreetName,
                    Version = h.Version,
                    ZipCode = h.Address.ZipCode,
                    Name = h.Name,
                    PhoneNumber = h.PhoneNumber,
                    DistanceToDepot = h.DistanceFromHub,
                    DefaultGate = h.DefaultGate != null ? new GateModel()
                    {
                        Id = h.DefaultGate.Id,
                        Number = h.DefaultGate.Number,
                        Version = h.DefaultGate.Version
                    } : null,
                    DefaultTrailer = h.DefaultTrailer != null ? new TrailerTypeModel()
                    {
                        Id = h.DefaultTrailer.Id,
                        Name = h.DefaultTrailer.Name,
                        Version = h.DefaultTrailer.Version
                    } : null
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Result<List<DepotModel>>(result);
        }
    }
}