using AutoFixture;
using FluentAssertions;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class DepotTests : SqlLiteInMemoryDbFixture
    {
        public ITestOutputHelper Output { get; }
        public Fixture Fixture { get; set; }
        public Random Rand { get; set; } = new Random(DateTime.Now.Millisecond);

        public DepotTests(ITestOutputHelper output)
        {
            Output = output;
            Fixture = new Fixture();
            Fixture.Register<Depot>(() =>
                new Depot(Fixture.Create<int>(),
                "name" + DateTime.Now.TimeOfDay,
                Rand.Next(100000, 9999999).ToString(),
                "email@wp.pl",
                Address.Create(Rand.Next(1, 900000).ToString(),
                    Rand.Next(10000, 99999).ToString(),
                    "aaa city",
                    "bbb country")));
        }

        [Fact]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await NewDbContext.Database.CanConnectAsync());
        }

        [Fact]
        public void Can_be_read_from_db()
        {
            var seedData = Fixture.CreateMany<Depot>(100);

            using (var ctx = NewDbContext)
            {
                ctx.Depots.AddRange(seedData);
                ctx.SaveChanges();
            }

            var ctxFactoryMock = new Mock<IContextFactory>();
            ctxFactoryMock.Setup(c => c.CreateDbContext()).Returns(NewDbContext);

            using var context = ctxFactoryMock.Object.CreateDbContext();
            var result = context.Depots.ToList();

            result.Should().BeEquivalentTo(seedData, o => o.Excluding(e => e.Version));
        }

        [Fact]
        public void Can_be_written_to_db()
        {
            var seedData = Fixture.CreateMany<Depot>(100);

            using (var ctx = NewDbContext)
            {
                ctx.Depots.AddRange(seedData);
                ctx.SaveChanges();
            }

            var subject = new Depot(999, "test_name", "123456",
                "em@wp.pl", Address.Create("street", "12345", "city", "country"));

            using (var context = NewDbContext)
            {
                context.Depots.Add(subject);
                context.SaveChanges();
            }

            Depot result = null;
            int count = 0;
            using (var contextRead = NewDbContext)
            {
                result = contextRead.Depots.FirstOrDefault(h => h.Id == 999);
                count = contextRead.Depots.Count();
            }

            result.Should().BeEquivalentTo(subject, o => o.Excluding(e => e.Version));
            count.Should().Be(101);
        }
    }
}