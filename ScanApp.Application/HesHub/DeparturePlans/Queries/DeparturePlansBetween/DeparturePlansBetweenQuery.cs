using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Helpers.EF_Queryable;
using static ScanApp.Domain.ValueObjects.RecurrencePattern;

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
                .AsNoTracking()
                .Where(x => (x.Start >= request.From && x.End <= request.To) || possibleOccurrences.Contains(x.Id))
                .Select(x => new DeparturePlanModel
                {
                    Gate = new GateModel { Id = x.Gate.Id, Name = x.Gate.Number.ToString(), Version = x.Gate.Version },
                    Trailer = new TrailerModel { Id = x.TrailerType.Id, Name = x.TrailerType.Name, Version = x.TrailerType.Version },
                    Season = x.Seasons.Select(s => new SeasonResourceModel()
                    {
                        Name = s.Name,
                        Version = s.Version
                    }).ToList(),
                    Version = x.Version,
                    ArrivalDayAndTime = x.ArrivalTimeAtDepot,
                    Id = x.Id,
                    Start = x.Start,
                    End = x.End,
                    Description = x.Description,
                    ExceptionToDate = x.RecurrenceExceptionDate,
                    Exceptions = x.RecurrenceExceptions.ToList(),
                    RecurrencePattern = x.RecurrencePattern,
                    ExceptionTo = x.RecurrenceExceptionOf == null ? null : new DeparturePlanModel
                    {
                        Gate = new GateModel { Id = x.RecurrenceExceptionOf.Gate.Id, Name = x.RecurrenceExceptionOf.Gate.Number.ToString(), Version = x.RecurrenceExceptionOf.Gate.Version },
                        Trailer = new TrailerModel { Id = x.RecurrenceExceptionOf.TrailerType.Id, Name = x.RecurrenceExceptionOf.TrailerType.Name, Version = x.RecurrenceExceptionOf.TrailerType.Version },
                        Season = x.RecurrenceExceptionOf.Seasons.Select(s => new SeasonResourceModel()
                        {
                            Name = s.Name,
                            Version = s.Version
                        }).ToList(),
                        Version = x.RecurrenceExceptionOf.Version,
                        ArrivalDayAndTime = x.RecurrenceExceptionOf.ArrivalTimeAtDepot,
                        Id = x.RecurrenceExceptionOf.Id,
                        Start = x.RecurrenceExceptionOf.Start,
                        End = x.RecurrenceExceptionOf.End,
                        Description = x.RecurrenceExceptionOf.Description,
                        Exceptions = x.RecurrenceExceptionOf.RecurrenceExceptions.ToList(),
                        RecurrencePattern = x.RecurrenceExceptionOf.RecurrencePattern,
                        ExceptionTo = null,
                        ExceptionToDate = null
                    }
                })
            .ToListAsync(cancellationToken).ConfigureAwait(false);

            return new Result<IEnumerable<DeparturePlanModel>>(depots);
        }
    }
}