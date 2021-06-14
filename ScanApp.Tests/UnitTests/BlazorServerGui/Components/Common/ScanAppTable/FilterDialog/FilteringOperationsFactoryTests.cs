using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ScanApp.Components.Common.ScanAppTable.FilterDialog;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.FilterDialog
{
    public class FilteringOperationsFactoryTests
    {
        private FilteringOperationsFactory CreateInstance()
        {
            var objectList = new List<FiltertingTestsFixture>();

            for (int i = 0; i < 50; i++)
            {
                var obj = new FiltertingTestsFixture
                {
                    Integer = i, 
                    Date = new DateTime(2021, 1, 1).AddDays(i),
                    String = "test" + i,
                    Decimal = i + (i/100)
                };

                objectList.Add(obj);
            }

            var properties = new FiltertingTestsFixture().GetType().GetProperties();

            var from = new int?[properties.Length];
            var to = new int?[properties.Length];
            var contains = new string[properties.Length];
            var fromDate = new DateTime?[properties.Length];
            var toDate = new DateTime?[properties.Length];
            var fromDecimal = new decimal?[properties.Length];
            var toDecimal = new decimal?[properties.Length];

            from[0] = 10;
            to[0] = 20;

            fromDate[1] = new DateTime(2021, 2, 1);
            toDate[1] = new DateTime(2021, 2, 10);

            contains[2] = "test5";

            fromDecimal[3] = new decimal(1.20);
            toDecimal[3] = new decimal(1.30);

            return new FilteringOperationsFactory(properties, from, to, contains, fromDate, toDate, fromDecimal, toDecimal);
        }


        [Fact]
        public void Will_create_instance()
        {
            var subject = CreateInstance();

            subject.Should().BeOfType<FilteringOperationsFactory>();
        }

        [Fact]
        public void Will_create_filtering_operations()
        {
            var subject = CreateInstance();
            var operationsList = subject.CreateOperations();

            Assert.Equal(4, operationsList.Count);
        }
    }
}
