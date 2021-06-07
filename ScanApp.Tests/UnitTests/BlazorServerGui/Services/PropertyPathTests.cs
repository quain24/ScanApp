using FluentAssertions;
using ScanApp.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Services
{
    public class PropertyPathTests
    {
        [Fact]
        public void Throws_arg_null_exc_if_expression_is_null()
        {
            Action act = () => PropertyPath<int>.GetFrom(null as Expression<Func<int, object>>);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(PropertyPathTestsFixtures.PropertyPathTheoryData))]
        public void Will_give_proper_path(Expression<Func<PropertyPathTestsFixtures.TestObject, object>> expr, IReadOnlyList<MemberInfo> infos)
        {
            var result = PropertyPath<PropertyPathTestsFixtures.TestObject>.GetFrom(expr);

            result.Should().HaveCount(infos.Count).And.BeEquivalentTo(infos, o =>
            {
                o.WithStrictOrdering();
                return o;
            });
        }

        [Fact]
        public void Returns_empty_collection_if_pointed_to_base_variable()
        {
            var result = PropertyPath<string>.GetFrom(s => s);
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_empty_collection_if_pointed_to_base_object()
        {
            var result = PropertyPath<PropertyPathTestsFixtures.TestObject>.GetFrom(s => s);
            result.Should().BeEmpty();
        }
    }
}