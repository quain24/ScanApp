using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots.Commands.EditDepot;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Application.Depots.EditDepot
{
    public class EditDepotCommandHandlerTests : SqlLiteInMemoryDbFixture
    {
        public EditDepotCommandHandlerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public async Task Will_change_id_and_update_plans_so_they_point_to_new_one()
        {
            var gate = new Gate(101, Gate.TrafficDirection.BiDirectional);
            var trailer = new TrailerType("basic trailer") { LoadingTime = TimeSpan.FromHours(10) };
            var season = new Season("Default", DateTime.MinValue, DateTime.MaxValue);
            var depot = new DepotDataFixtures.DepotBuilder().WithGate(gate).WithTrailerType(trailer).Build();
            var plan = new DeparturePlan("Default", depot, season, gate, trailer, DayAndTime.Now, TimeSpan.FromHours(12), DayAndTime.From(DayOfWeek.Friday, TimeSpan.FromHours(12)));

            await using (var ctx = NewDbContext)
            {
                ctx.Add(gate);
                ctx.Add(trailer);
                ctx.Add(season);
                ctx.SaveChanges();
                ctx.Entry(gate).State = EntityState.Detached;
                ctx.Entry(trailer).State = EntityState.Detached;
                ctx.Add(depot);
                ctx.Entry(depot.DefaultGate).State = EntityState.Unchanged;
                ctx.Entry(depot.DefaultTrailer).State = EntityState.Unchanged;
                ctx.SaveChanges();
                ctx.Add(plan);
                ctx.SaveChanges();
            }

            Output.WriteLine("\r\nDatabase seed finished =======================================\r\n");

            var orgModel = new DepotDataFixtures.DepotBuilder().BuildAsModel(depot);
            var editedModel = new DepotDataFixtures.DepotBuilder().BuildAsModel(depot);
            editedModel.Id = 128;

            var command = new EditDepotCommand(orgModel, editedModel);
            var subject = new EditDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            var result = await subject.Handle(command, CancellationToken.None);

            using var scope = new AssertionScope();
            await using (var cctx = NewDbContext)
            {
                result.Conclusion.Should().BeTrue();
                result.ResultType.Should().Be(ResultType.Updated);

                cctx.Depots.Should().HaveCount(1, "new depot was added, old was deleted");
                var editedDepot = cctx.Depots
                    .Include(x => x.DefaultGate)
                    .Include(x => x.DefaultTrailer)
                    .FirstOrDefault(x => x.Id == editedModel.Id);
                editedDepot.Should().NotBeNull("only depot left is the one with changed ID")
                    .And.Subject.As<Depot>().Id.Should().Be(editedModel.Id, "new ID must equals to given ID");

                cctx.Gates.Should().HaveCount(1, "no duplication of gates should occur")
                    .And.Subject.First().Should().BeEquivalentTo(gate, "gate was not changed");
                cctx.TrailerTypes.Should().HaveCount(1, "no duplication of trailer types should occur")
                    .And.Subject.First().Should().BeEquivalentTo(trailer, "trailer was not changed");

                cctx.DeparturePlans.Should().HaveCount(1, "departure plan had it's depot field changed, so there still should be only one");

                var editedPlan = cctx.DeparturePlans.Include(x => x.Depot).Include(x => x.Seasons).First();

                editedPlan.Should().BeEquivalentTo(plan, o =>
                {
                    o.Excluding(x => x.Depot.Version); // It is a "new" depot - Key was changed.
                    o.Excluding(x => x.Depot.Id);      // Id / Key was changed in new depot.
                    o.IgnoringCyclicReferences();
                    return o;
                }, "Only id and version was changed");

                editedPlan.Depot.Id.Should().Be(editedModel.Id);
            }
        }
    }
}