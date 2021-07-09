using FluentAssertions;
using ScanApp.Common.Helpers;
using System.Collections.Generic;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Helpers
{
    public class WordAndNumberStringComparerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new WordAndNumberStringComparer();

            subject.Should().NotBeNull()
                .And.BeOfType<WordAndNumberStringComparer>()
                .And.BeAssignableTo<IComparer<string>>();
        }

        [Theory]
        [InlineData("1", "A")]
        [InlineData("2", "A")]
        [InlineData("1", "B")]
        [InlineData("12", "AAAA")]
        [InlineData("1", "1A")]
        public void Number_always_wins(string left, string right)
        {
            var subject = new WordAndNumberStringComparer();
            var result = subject.Compare(left, right);

            result.Should().Be(1, "left is always greater in this test");
        }

        [Fact]
        public void Nulls_are_eq()
        {
            var subject = new WordAndNumberStringComparer();
            var result = subject.Compare(null, null);

            result.Should().Be(0);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("A")]
        public void Null_vs_non_null_null_is_always_less(string value)
        {
            var subject = new WordAndNumberStringComparer();
            var result = subject.Compare(value, null);

            result.Should().Be(1);
        }
    }
}