using EntityFramework.Exceptions.Common;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot;
using ScanApp.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.IntegrationTests.Application.Depots.CreateNewDepot
{
    public class CreateNewDepotCommandHandlerTests : SqlLiteInMemoryDbFixture
    {
        [Fact]
        public async Task Will_add_depot_with_proper_relation_data()
        {
            var gate = new Gate(0, Gate.TrafficDirection.Incoming);
            var trailer = new TrailerType("trailer");

            using (var ctx = NewDbContext)
            {
                ctx.Gates.Add(gate);
                ctx.TrailerTypes.Add(trailer);
                await ctx.SaveChangesAsync();
            }

            var model = new DepotDataFixtures
                .DepotBuilder()
                .WithGate(gate)
                .WithTrailerType(trailer)
                .BuildAsModel();

            var subject = new CreateNewDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            var result = await subject.Handle(new CreateNewDepotCommand(model), CancellationToken.None);

            result.Conclusion.Should().BeTrue();

            using var scope = new AssertionScope();
            using var cctx = NewDbContext;
            cctx.Depots.Should().HaveCount(1);
            var depot = cctx.Depots.Include(x => x.DefaultGate).Include(x => x.DefaultTrailer)
                .FirstOrDefault();

            depot.Should().BeEquivalentTo(new DepotDataFixtures.DepotBuilder()
                .WithGate(gate)
                .WithTrailerType(trailer).Build(), o => o.Excluding(x => x.Version));
        }

        [Fact]
        public async Task Will_throw_reference_constraint_if_given_non_existing_gate()
        {
            using (var ctx = NewDbContext)
            {
                ctx.Gates.Add(new Gate(0, Gate.TrafficDirection.Incoming));
                ctx.TrailerTypes.Add(new TrailerType("trailer"));
                await ctx.SaveChangesAsync();
            }

            var model = new DepotDataFixtures
                .DepotBuilder()
                .WithGate(new Gate(12, Gate.TrafficDirection.BiDirectional) { Id = 100, Version = Version.Empty })
                .BuildAsModel();

            var subject = new CreateNewDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            Func<Task> act = async () => _ = await subject.Handle(new CreateNewDepotCommand(model), CancellationToken.None);

            await act.Should().ThrowAsync<ReferenceConstraintException>();
        }

        [Fact]
        public async Task Will_throw_reference_constraint_if_given_non_existing_trailer_type()
        {
            using (var ctx = NewDbContext)
            {
                ctx.Gates.Add(new Gate(0, Gate.TrafficDirection.Incoming));
                ctx.TrailerTypes.Add(new TrailerType("trailer"));
                await ctx.SaveChangesAsync();
            }

            var model = new DepotDataFixtures
                .DepotBuilder()
                .WithTrailerType(new TrailerType("trailer one") { Id = 100, Version = Version.Empty })
                .BuildAsModel();

            var subject = new CreateNewDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            Func<Task> act = async () => _ = await subject.Handle(new CreateNewDepotCommand(model), CancellationToken.None);

            await act.Should().ThrowAsync<ReferenceConstraintException>();
        }

        [Fact]
        public async Task Gate_and_trailer_are_optional()
        {
            using (var ctx = NewDbContext)
            {
                ctx.Gates.Add(new Gate(0, Gate.TrafficDirection.Incoming));
                ctx.TrailerTypes.Add(new TrailerType("trailer"));
                await ctx.SaveChangesAsync();
            }

            var model = new DepotDataFixtures.DepotBuilder().BuildAsModel();

            var subject = new CreateNewDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            var result = await subject.Handle(new CreateNewDepotCommand(model), CancellationToken.None);

            result.Conclusion.Should().BeTrue();

            using var scope = new AssertionScope();
            using var cctx = NewDbContext;
            cctx.Depots.Should().HaveCount(1);
            var depot = cctx.Depots.Include(x => x.DefaultGate).Include(x => x.DefaultTrailer)
                .FirstOrDefault();

            depot.Should().BeEquivalentTo(new DepotDataFixtures.DepotBuilder().Build(), o =>
            {
                o.Excluding(x => x.Version);
                return o;
            });
        }

        [Fact]
        public async Task Will_throw_unique_constraint_exc_if_given_depot_with_id_already_existing_in_db()
        {
            using (var ctx = NewDbContext)
            {
                ctx.Gates.Add(new Gate(0, Gate.TrafficDirection.Incoming));
                ctx.TrailerTypes.Add(new TrailerType("trailer"));
                ctx.Depots.Add(new DepotDataFixtures.DepotBuilder().WithId(100).Build());
                await ctx.SaveChangesAsync();
            }

            var model = new DepotDataFixtures
                .DepotBuilder()
                .WithId(100)
                .WithName("new name")
                .BuildAsModel();

            var subject = new CreateNewDepotCommandHandler(Provider.GetRequiredService<IContextFactory>());

            Func<Task> act = async () => _ = await subject.Handle(new CreateNewDepotCommand(model), CancellationToken.None);

            await act.Should().ThrowAsync<UniqueConstraintException>();
        }
    }
}