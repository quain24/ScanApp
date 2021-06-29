using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Queries.AllSparePartTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.Hubs.Queries.AllHubs
{
    public record AllHubsQuery : IRequest<Result<List<HesHubModel>>>;

    internal class AllHubsQueryHandler : IRequestHandler<AllHubsQuery, Result<List<HesHubModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public AllHubsQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<HesHubModel>>> Handle(AllHubsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var result = ctx.HesDepots.AsNoTracking().Select(h => new HesHubModel()
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

                return new Result<List<HesHubModel>>(result);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<HesHubModel>>(ErrorType.Cancelled, ex);
            }
            catch (SqlException ex)
            {
                return new Result<List<HesHubModel>>(ErrorType.DatabaseError, ex?.InnerException?.Message, ex);
            }
        }
    }
}