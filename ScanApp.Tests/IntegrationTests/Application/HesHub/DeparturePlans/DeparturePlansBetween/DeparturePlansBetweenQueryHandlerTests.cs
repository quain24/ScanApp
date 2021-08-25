using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween;
using ScanApp.Domain.Entities;
using ScanApp.Domain.Enums;
using ScanApp.Domain.ValueObjects;
using ScanApp.Services;
using ScanApp.Tests.UnitTests.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using OccurrenceCalculatorService = ScanApp.Services.OccurrenceCalculatorService;

namespace ScanApp.Tests.IntegrationTests.Application.HesHub.DeparturePlans.DeparturePlansBetween
{
    public class DeparturePlansBetweenQueryHandlerTests : SqlLiteInMemoryDbFixture
    {
        public DeparturePlansBetweenQueryHandlerTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public async Task Will_retrieve_correct_models()
        {
            var startDate = new DateTime(2000, 04, 24, 11, 35, 00, DateTimeKind.Utc);
            var endDate = new DateTime(2000, 04, 24, 12, 35, 00, DateTimeKind.Utc);
            var startExceptionDate = new DateTime(2000, 08, 24, 11, 35, 00, DateTimeKind.Utc);
            var endExceptionDate = new DateTime(2000, 08, 24, 12, 35, 00, DateTimeKind.Utc);
            var recurrence = RecurrencePattern.Daily(2, 4);

            var depot = new DepotFixtures.DepotBuilder()
                .WithId(10).WithDefaultValidAddress()
                .WithName("depot name")
                .WithPhoneNumber("123654")
                .WithEmail("wp@wp.pl")
                .Build();
            var season = new Season("season", DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime());
            var gate = new Gate(10, Gate.TrafficDirection.Outgoing);
            var trailer = new TrailerType("standard");

            var recurringOcc = new DeparturePlan("recc model", startDate, endDate, depot, season, gate, trailer, DayAndTime.Now)
            {
                RecurrencePattern = recurrence
            };

            var exceptionOcc = new DeparturePlan("exce model", startExceptionDate, endExceptionDate, depot, season, gate, trailer, DayAndTime.Now);

            recurringOcc.AddRecurrenceException(exceptionOcc, startExceptionDate);

            using (var ctx = NewDbContext)
            {
                ctx.Add(gate);
                ctx.Add(trailer);
                ctx.Add(season);
                ctx.SaveChanges();
                ctx.Add(depot);
                depot.DefaultGate = gate;
                depot.DefaultTrailer = trailer;
                ctx.SaveChanges();
                ctx.AddRange(recurringOcc, exceptionOcc);
                ctx.SaveChanges();
            }

            var startPeriod = new DateTime(2000, 10, 24, 11, 35, 00, DateTimeKind.Utc);
            var endPeriod = new DateTime(2000, 11, 24, 12, 35, 00, DateTimeKind.Utc);
            var request = new DeparturePlansBetweenQuery(startPeriod, endPeriod);

            var handler = new DeparturePlansBetweenQueryHandler(Provider.GetRequiredService<IContextFactory>());

            var result = await handler.Handle(request, CancellationToken.None);

            var map = new RecurrenceSyncfusionMapper();
            var srv = new OccurrenceCalculatorService();
            var g = map.ToSyncfusionRule(recurrence);

            var dd = startDate + TimeSpan.FromDays(21);
            var iss = srv.GetOccurrenceDates(recurrence, startDate, dd);

            var t = result.Output;
            t = result.Output;
        }
    }
}