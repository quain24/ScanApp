using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using ScanApp.Components.Table.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Table.Utilities
{
    public class FiltererTests
    {
        public ITestOutputHelper Output { get; }

        public FiltererTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Throws_arg_null_exc_when_called_on_null_collection()
        {
            Action act = () => _ = (null as IEnumerable<string>).Filter(Array.Empty<IFilter<string>>());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_one_of_filters_is_null()
        {
            var source = new string[0];
            var filters = new[] { Mock.Of<IFilter<string>>(), null };
            Action act = () => _ = source.Filter(filters);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Returns_source_collection_if_there_are_no_filters()
        {
            var source = new string[0];
            var result = source.Filter(Array.Empty<IFilter<string>>());

            result.Should().BeSameAs(source);
        }

        [Fact]
        public void Uses_Run_method_if_there_is_only_one_filter()
        {
            var filterMock = new Mock<IFilter<string>>();
            filterMock.Setup(x => x.Check(It.IsAny<string>())).Returns(true);
            var source = new string[2] { "a", "b" };
            var _ = source.Filter(new List<IFilter<string>> { filterMock.Object });

            filterMock.Verify(x => x.Run(It.Is<IEnumerable<string>>(s => s == source)), Times.Once);
            filterMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Uses_Check_method_if_there_is_more_then_one_filter()
        {
            var filterMock = new Mock<IFilter<string>>();
            var filterMock2 = new Mock<IFilter<string>>();
            filterMock.Setup(x => x.Check(It.Is<string>(s => s == "a" || s == "c"))).Returns(true).Callback<string>(s => Output.WriteLine("f: " + s));
            filterMock.Setup(x => x.Check(It.Is<string>(s => s == "b"))).Returns(false).Callback<string>(s => Output.WriteLine("f: " + s));
            filterMock2.Setup(x => x.Check(It.Is<string>(s => s == "b" || s == "c"))).Returns(true).Callback<string>(s => Output.WriteLine("s: " + s));
            filterMock2.Setup(x => x.Check(It.Is<string>(s => s == "a"))).Returns(false).Callback<string>(s => Output.WriteLine("s: " + s));
            var source = new string[3] { "a", "b", "c" };
            var result = source.Filter(new List<IFilter<string>> { filterMock.Object, filterMock2.Object });

            using var assertionScope = new AssertionScope();
            filterMock.Verify(x => x.Check(It.IsAny<string>()), Times.AtLeastOnce);
            filterMock2.Verify(x => x.Check(It.Is<string>(s => s == "b" || s == "c")), Times.AtLeastOnce);
            filterMock.VerifyNoOtherCalls();

            result.Should().HaveCount(1)
                .And.Subject.First().Should().Be("c");
        }
    }
}