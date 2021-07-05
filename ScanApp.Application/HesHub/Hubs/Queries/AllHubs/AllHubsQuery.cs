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

namespace ScanApp.Application.HesHub.Hubs.Queries.AllHubs
{
    public record AllHubsQuery : IRequest<Result<List<DepotModel>>>;

    internal class AllHubsQueryHandler : IRequestHandler<AllHubsQuery, Result<List<DepotModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public AllHubsQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<DepotModel>>> Handle(AllHubsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var result = ctx.Depots.AsNoTracking().Select(h => new DepotModel()
                {
                    City = h.Address.City,
                    Country = h.Address.Country,
                    Email = h.Email,
                    Id = h.Id,
                    StreetName = h.Address.StreetName,
                    Version = h.Version,
                    ZipCode = h.Address.ZipCode,
                    StreetNumber = h.Address.StreetNumber,
                    Name = h.Name,
                    PhoneNumber = h.PhoneNumber,
                    PhonePrefix = h.PhonePrefix
                }).ToList();

                return new Result<List<DepotModel>>(result);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<DepotModel>>(ErrorType.Cancelled, ex);
            }
            catch (SqlException ex)
            {
                return new Result<List<DepotModel>>(ErrorType.DatabaseError, ex?.InnerException?.Message, ex);
            }
        }
    }
}