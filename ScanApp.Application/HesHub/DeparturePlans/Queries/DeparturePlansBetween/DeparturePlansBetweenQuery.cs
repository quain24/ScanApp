using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween
{
    public record DeparturePlansBetweenQuery(DateTime From, DateTime To) : IRequest<Result<IEnumerable<DeparturePlanModel>>>;

    internal class DeparturePlansBetweenQueryHandler : IRequestHandler<DeparturePlansBetweenQuery, Result<IEnumerable<DeparturePlanModel>>>
    {
        private readonly IContextFactory _factory;
        private readonly IRecurrenceCheck _recurrenceCheck;

        public DeparturePlansBetweenQueryHandler(IContextFactory factory, IRecurrenceCheck recurrenceCheck)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _recurrenceCheck = recurrenceCheck ?? throw new ArgumentNullException(nameof(recurrenceCheck));
        }

        public async Task<Result<IEnumerable<DeparturePlanModel>>> Handle(DeparturePlansBetweenQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();
            var possibleOccurrences = await _recurrenceCheck
                .GetPossibleRecurrencesIds(ctx.DeparturePlans, request.From, request.To, cancellationToken)
                .ConfigureAwait(false);

            var depots = await ctx.DeparturePlans
                .AsNoTrackingWithIdentityResolution()
                //.AsSplitQuery() - this is pushed to EF Core 6 - stopped working.
                .Include(x => x.RecurrenceExceptionOf)
                .Include(x => x.Gate)
                .Include(x => x.TrailerType)
                .Include(x => x.Seasons)
                .Where(x => (x.Start >= request.From && x.End <= request.To) || possibleOccurrences.Contains(x.Id))
                .Select(DeparturePlanModel.Projection)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return new Result<IEnumerable<DeparturePlanModel>>(depots);
        }
    }
}