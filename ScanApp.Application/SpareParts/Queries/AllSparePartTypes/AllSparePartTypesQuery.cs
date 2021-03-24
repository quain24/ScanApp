using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.AllSparePartTypes
{
    public class AllSparePartTypesQuery : IRequest<Result<List<SparePartTypeModel>>>
    {
        public class AllSparePartTypesQueryHandler : IRequestHandler<AllSparePartTypesQuery, Result<List<SparePartTypeModel>>>
        {
            private readonly IApplicationDbContext _context;

            public AllSparePartTypesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Result<List<SparePartTypeModel>>> Handle(AllSparePartTypesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var parts = await _context.SparePartTypes
                        .AsNoTracking()
                        .Select(s => new SparePartTypeModel(s.Name))
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return new Result<List<SparePartTypeModel>>(parts);
                }
                catch (Exception ex)
                {
                    return ex is DbUpdateConcurrencyException
                        ? new Result<List<SparePartTypeModel>>(ErrorType.ConcurrencyFailure, ex)
                        : new Result<List<SparePartTypeModel>>(ErrorType.Unknown, ex);
                }
            }
        }
    }
}