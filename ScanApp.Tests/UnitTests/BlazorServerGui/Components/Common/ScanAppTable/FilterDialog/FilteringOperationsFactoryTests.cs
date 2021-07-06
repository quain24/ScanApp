using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ScanApp.Components.Common.ScanAppTable.FilterDialog;
using ScanApp.Components.Common.ScanAppTable.Options;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.FilterDialog
{
    public class FilteringOperationsFactoryTests
    {
        private FilteringOperationsFactory<FiltertingTestsFixture> CreateInstance()
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

            var columnConfigs = new List<ColumnConfiguration<FiltertingTestsFixture>>();
            columnConfigs.Add(new ColumnConfiguration<FiltertingTestsFixture>(x => x.Decimal, "Decimal"));
            columnConfigs.Add(new ColumnConfiguration<FiltertingTestsFixture>(x => x.Date, "Date"));
            columnConfigs.Add(new ColumnConfiguration<FiltertingTestsFixture>(x => x.Integer, "Integer"));
            columnConfigs.Add(new ColumnConfiguration<FiltertingTestsFixture>(x => x.String, "String"));


            var from = new int?[columnConfigs.Count];
            var to = new int?[columnConfigs.Count];
            var contains = new string[columnConfigs.Count];
            var fromDate = new DateTime?[columnConfigs.Count];
            var toDate = new DateTime?[columnConfigs.Count];
            var fromDecimal = new decimal?[columnConfigs.Count];
            var toDecimal = new decimal?[columnConfigs.Count];

            from[0] = 10;
            to[0] = 20;

            fromDate[1] = new DateTime(2021, 2, 1);
            toDate[1] = new DateTime(2021, 2, 10);

            contains[2] = "test5";

            fromDecimal[3] = new decimal(1.20);
            toDecimal[3] = new decimal(1.30);

            return new FilteringOperationsFactory<FiltertingTestsFixture>(columnConfigs, from, to, contains, fromDate, toDate, fromDecimal, toDecimal);
        }


        [Fact]
        public void Will_create_instance()
        {
            FilteringOperationsFactory<FiltertingTestsFixture> subject = CreateInstance();

            subject.Should().BeOfType<FilteringOperationsFactory<FiltertingTestsFixture>>();
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
