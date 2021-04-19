﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Queries.AllSparePartTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.GetAllStoragePlaces
{
    public record GetAllStoragePlacesQuery : IRequest<Result<List<RepairWorkshopModel>>>;

    internal class GetAllStoragePlacesQueryHandler : IRequestHandler<GetAllStoragePlacesQuery, Result<List<RepairWorkshopModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public GetAllStoragePlacesQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<List<RepairWorkshopModel>>> Handle(GetAllStoragePlacesQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            try
            {
                var places = await ctx.SparePartStoragePlaces
                    .AsNoTracking()
                    .Select(e => new RepairWorkshopModel { Number = e.Name, Id = e.Id })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Result<List<RepairWorkshopModel>>(places);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<RepairWorkshopModel>>(ErrorType.Cancelled, ex);
            }
        }
    }
}