using FluentAssertions;
using ScanApp.Common.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Extensions
{
    public class ArrayExtensionsTests
    {
        private int[] TestData { get; } = { 1, 2, 3, 4 };

        [Fact]
        public void Throws_arg_null_exc_when_run_on_null_array()
        {
            Action act = () => (null as int[]).ForEach((x, y) => _ = (int)x.GetValue(y));

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Traverses_standard_array()
        {
            var result = new List<int>();
            TestData.ForEach((x, y) => result.Add((int)x.GetValue(y)));
            result.Should().BeEquivalentTo(TestData);
        }
    }
}