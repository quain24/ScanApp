using FluentAssertions;
using ScanApp.Common.Helpers;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Helpers
{
    public class LineNumberTests
    {
        [Fact]
        public void Gets_file_lineNumber_on_which_it_was_called()
        {
            var result = LineNumber.Get();
            result.Should().Be(12);
        }
    }
}