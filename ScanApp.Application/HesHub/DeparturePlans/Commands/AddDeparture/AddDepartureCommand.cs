using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.HesHub.DeparturePlans.Commands.AddDeparture
{
    public record AddDepartureCommand(DeparturePlanModel DeparturePlan) : IRequest<Result>;

    internal class AddDepartureCommandHandler : IRequestHandler<AddDepartureCommand, Result>
    {
        private readonly IContextFactory _factory;

        public AddDepartureCommandHandler(IContextFactory factory)
        {
            _factory = factory;
        }
        public async Task<Result> Handle(AddDepartureCommand request, CancellationToken cancellationToken)
        {
            await using var ctx = _factory.CreateDbContext();

            var newDeparturePlan = request.DeparturePlan;
            var conflict = await ctx.DeparturePlans
                .Include(x => x.Gate) 
                .Include(x => x.Seasons)
                .Where(c => c.Gate == null ? newDeparturePlan.GateId == null : c.Gate.Id == newDeparturePlan.GateId)
                .Where(c => c.Start <= newDeparturePlan.End)
                .Where(c => c.Seasons.Any(x => newDeparturePlan.Seasons.Any(b => b == x.Name)))
                .FirstOrDefaultAsync();

            if (conflict is not null)
                return new Result(ErrorType.Duplicated, $"Departure plan overlaps with one or more other plans - {conflict.Id}");

            var seasons = await ctx.Seasons
                .Where(x => newDeparturePlan.Seasons.Any(c => c == x.Name))
                .ToListAsync(cancellationToken);
            var depot = await ctx.Depots
                .Where(x => x.Id == newDeparturePlan.DepotId)
                .FirstOrDefaultAsync(cancellationToken);
            var gate = await ctx.Gates.FirstOrDefaultAsync(x => x.Id == newDeparturePlan.GateId);

            var trailer = await ctx.TrailerTypes.FirstOrDefaultAsync(x => x.Id == newDeparturePlan.TrailerId);
            trailer ??= await ctx.TrailerTypes.FirstAsync();

            var plan = new DeparturePlan(newDeparturePlan.Subject, newDeparturePlan.Start, newDeparturePlan.End, depot,
                seasons, gate, trailer, DayAndTime.Now);

            ctx.DeparturePlans.Attach(plan);
            ctx.Entry(plan).State = EntityState.Added;
            //foreach (var planSeason in plan.Seasons)
            //{
            //    ctx.Entry(planSeason).State = EntityState.Detached;
            //}

            //ctx.Entry(plan.Depot).State = EntityState.Detached;
            //ctx.Entry(plan.Gate).State = EntityState.Detached;
            //ctx.Entry(plan.TrailerType).State = EntityState.Detached;
            try
            {
                var t = await ctx.SaveChangesAsync();
            }
            catch(Exception e)
            {
                var a = "";
            }
            return new Result();
        }
    }
}
