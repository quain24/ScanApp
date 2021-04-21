﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.SpareParts.Queries.GetAllSparePartStoragePlaces
{
    public record GetAllSparePartStoragePlacesQuery : IRequest<Result<List<RepairWorkshopModel>>>;

    internal class GetAllSparePartStoragePlacesQueryHandler : IRequestHandler<GetAllSparePartStoragePlacesQuery, Result<List<RepairWorkshopModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public GetAllSparePartStoragePlacesQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<RepairWorkshopModel>>> Handle(GetAllSparePartStoragePlacesQuery request, CancellationToken cancellationToken)
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