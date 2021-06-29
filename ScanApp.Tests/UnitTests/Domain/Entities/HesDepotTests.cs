using AutoFixture;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class HesDepotTests : SqlLiteInMemoryDbFixture
    {
        public ITestOutputHelper Output { get; }
        public Fixture Fixture { get; set; }
        public Random Rand { get; set; } = new Random(DateTime.Now.Millisecond);

        public HesDepotTests(ITestOutputHelper output)
        {
            Output = output;
            Fixture = new Fixture();
            Fixture.Register<HesDepot>(() =>
                new HesDepot(Fixture.Create<int>(),
                "name" + DateTime.Now.TimeOfDay,
                Address.Create(Rand.Next(1, 900000).ToString(),
                    "12",
                    Rand.Next(10000, 99999).ToString(),
                    "aaa city",
                    "bbb country"),
                Rand.Next(100, 10000).ToString(),
                Rand.Next(100000, 9999999).ToString(),
                "email@wp.pl"));
        }

        [Fact]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await NewDbContext.Database.CanConnectAsync());
        }

        [Fact]
        public void Can_be_read_from_db()
        {
            var seedData = Fixture.CreateMany<HesDepot>(100);

            using (var ctx = NewDbContext)
            {
                ctx.HesDepots.AddRange(seedData);
                ctx.SaveChanges();
            }

            var ctxFactoryMock = new Mock<IContextFactory>();
            ctxFactoryMock.Setup(c => c.CreateDbContext()).Returns(NewDbContext);

            using var context = ctxFactoryMock.Object.CreateDbContext();
            var result = context.HesDepots.ToList();

            result.Should().BeEquivalentTo(seedData);
        }

        [Fact]
        public void Can_be_written_to_db()
        {
            var seedData = Fixture.CreateMany<HesDepot>(100);

            using (var ctx = NewDbContext)
            {
                ctx.HesDepots.AddRange(seedData);
                ctx.SaveChanges();
            }

            var subject = new HesDepot(999, "test_name",
                Address.Create("street", "2A", "12345", "city", "country"),
                "12pref", "123456", "em@wp.pl");

            using (var context = NewDbContext)
            {
                context.HesDepots.Add(subject);
                context.SaveChanges();
            }

            HesDepot result = null;
            int count = 0;
            using (var contextRead = NewDbContext)
            {
                result = contextRead.HesDepots.Where(h => h.Id == 999).FirstOrDefault();
                count = contextRead.HesDepots.Count();
            }

            result.Should().BeEquivalentTo(subject);
            count.Should().Be(101);
        }
    }
}