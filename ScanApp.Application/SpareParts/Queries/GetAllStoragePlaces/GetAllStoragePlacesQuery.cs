﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.GetAllStoragePlaces
{
    public class GetAllStoragePlacesQuery : IRequest<Result<List<RepairWorkshopModel>>>
    {
        public class GetAllStoragePlacesQueryHandler : IRequestHandler<GetAllStoragePlacesQuery, Result<List<RepairWorkshopModel>>>
        {
            private readonly IApplicationDbContext _context;

            public GetAllStoragePlacesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Result<List<RepairWorkshopModel>>> Handle(GetAllStoragePlacesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var places = await _context.StoragePlaces
                        .AsNoTracking()
                        .Select(e => new RepairWorkshopModel { Number = e.Name, Id = e.Id })
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return new Result<List<RepairWorkshopModel>>(places);
                }
                catch (Exception ex)
                {
                    return ex is DbUpdateConcurrencyException
                        ? new Result<List<RepairWorkshopModel>>(ErrorType.ConcurrencyFailure, ex)
                        : new Result<List<RepairWorkshopModel>>(ErrorType.Unknown, ex);
                }
            }
        }
    }
}