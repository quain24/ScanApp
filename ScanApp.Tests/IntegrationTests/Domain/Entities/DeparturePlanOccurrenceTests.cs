using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Domain.Entities
{
    public class DeparturePlanOccurrenceTests : SqlLiteInMemoryDbFixture
    {
        public DeparturePlanOccurrenceTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public async Task test()
        {
            //var depot = new DepotDataFixtures.DepotBuilder().Build();
            //var plan = new DeparturePlan("name", depot,
            //    new Season("season name", DateTime.Now, DateTime.Now + TimeSpan.FromHours(1)),
            //    new Gate(10, Gate.TrafficDirection.BiDirectional), new TrailerType("trailer name"), DayAndTime.Now,
            //    TimeSpan.FromHours(1), DayAndTime.Now);

            //using (var t = NewDbContext)
            //{
            //    t.DeparturePlanOccurrences.Add(new DeparturePlanOccurrence()
            //    {
            //        Start = DateTime.Now,
            //        End = DateTime.Now + TimeSpan.FromDays(1),
            //        OccurrenceOf = plan
            //    });

            //    t.SaveChanges();
            //}

            //using (var r = NewDbContext)
            //{
            //    var dd = r.DeparturePlanOccurrences.FirstOrDefault();

            //    dd.AddException(new DeparturePlanOccurrenceExceptionCase()
            //    {
            //        Start = DateTime.Now,
            //        End = DateTime.Now + TimeSpan.FromHours(1),
            //        Type = Type.Modified
            //    });

            //    r.SaveChanges();
            //}

            //using (var u = NewDbContext)
            //{
            //    var dep = u.DeparturePlanOccurrences.Include(x => x.OccurrenceOf).First();
            //    var del = u.DeparturePlans.Where(x => x.Name.Equals("name")).First();
            //    u.DeparturePlans.Remove(del);
            //    u.SaveChanges();
            //    var t = "";
            //}
        }
    }
}