using System;
using System.Collections.Generic;
using ScanApp.Infrastructure.Persistence;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Infrastructure.Persistence.Extensions
{
    public class DateTimeListToUtcStringConverterTests
    {
        public ITestOutputHelper Output { get; }

        public DateTimeListToUtcStringConverterTests(ITestOutputHelper output)
        {
            Output = output;
        }
        [Fact]
        public void test()
        {
            var aa = TimeZoneInfo.Local;
            var subject = new DateTimeListToUtcStringConverter();
            Output.WriteLine(aa.BaseUtcOffset.ToString() + "\r\n");
            Output.WriteLine(aa.GetUtcOffset(DateTime.Now).ToString() + "\r\n");
            Output.WriteLine(aa.GetUtcOffset(DateTime.Now - TimeSpan.FromDays(0)).ToString() + "\r\n");
            Output.WriteLine(DateTime.Now.ToUniversalTime().ToString() + "\r\n");
            Output.WriteLine(DateTime.Now.ToUniversalTime().ToString("s") + "\r\n");
            string a = subject.ConvertToProvider(new List<DateTime>() { DateTime.Now.ToUniversalTime(), DateTime.UtcNow, new DateTime(2021, 08, 24, 11, 24, 00, DateTimeKind.Utc) }) as string;
            Output.WriteLine(a);

            var b = subject.ConvertFromProvider(a) as List<DateTime>;
            b.ForEach(x =>
            {
                Output.WriteLine(x.ToString("s"));
                Output.WriteLine(x.Kind.ToString());
                Output.WriteLine("\r\n");
            });
        }
    }
}
