using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots.Commands.DeleteDepot;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.IntegrationTests.Application.Depots.DeleteDepot
{
    public class DeleteDepotCommandHandlerTests : SqlLiteInMemoryDbFixture
    {
        [Fact]
        public async Task Will_remove_existing_depot()
        {
            var depots = new List<Depot>
            {
                new DepotDataFixtures.DepotBuilder().Build(),
                new DepotDataFixtures.DepotBuilder().WithName("new name").WithEmail("email@wp.pl").WithId(1).Build()
            };

            var gate = new Gate(10, Gate.TrafficDirection.Incoming) { Version = Version.Empty };
            var trailer = new TrailerType("trailer") { MaxWeight = 1250 };

            using (var ctx = NewDbContext)
            {
                ctx.Gates.Add(gate);
                ctx.TrailerTypes.Add(trailer);
                ctx.SaveChanges();
                depots.First().DefaultGate = gate;
                depots.First().DefaultTrailer = trailer;
                ctx.Depots.AddRange(depots);
                ctx.SaveChanges();
            }

            var command = new DeleteDepotCommand(depots.First().Id, depots.First().Version);
            var subject = new DeleteDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            var result = await subject.Handle(command, CancellationToken.None);

            using (var scope = new AssertionScope())
            {
                result.Conclusion.Should().BeTrue();
                result.ResultType.Should().Be(ResultType.Deleted);
                using var cctx = NewDbContext;
                cctx.Depots.Should().HaveCount(1).And.NotContain(x => x.Id == depots.First().Id);
            }
        }
    }
}