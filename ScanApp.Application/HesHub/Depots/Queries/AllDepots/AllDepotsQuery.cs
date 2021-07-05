using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;

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
                    Name = h.Name,
                    PhoneNumber = h.PhoneNumber,
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