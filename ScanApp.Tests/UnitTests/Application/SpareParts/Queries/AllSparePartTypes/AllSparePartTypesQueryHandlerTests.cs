using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.SpareParts.Queries.AllSparePartTypes;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Queries.AllSparePartTypes
{
    public class AllSparePartTypesQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new AllSparePartTypesQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<AllSparePartTypesQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_if_no_context_factory_is_provided()
        {
            Action act = () => _ = new AllSparePartTypesQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_all_spare_part_types_as_valid_result_containing_SparePartTypeModel_collection()
        {
            var data = new List<SparePartType>
            {
                new("part_a"),
                new("part_b"),
                new("part_c")
            };
            var dataMock = data.AsQueryable().BuildMockDbSet();
            ContextMock.SetupGet(c => c.SparePartTypes).Returns(dataMock.Object);

            var subject = new AllSparePartTypesQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new AllSparePartTypesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(data);
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            ContextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new AllSparePartTypesQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new AllSparePartTypesQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(OperationCanceledException))]
        [InlineData(typeof(TaskCanceledException))]
        public async Task Returns_invalid_result_of_cancelled_on_cancellation_or_timeout(Type type)
        {
            dynamic exc = Activator.CreateInstance(type);
            ContextFactoryMock.Setup(m => m.CreateDbContext()).Throws(exc);

            var subject = new AllSparePartTypesQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new AllSparePartTypesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Canceled);
            result.ErrorDescription.Exception.Should().BeOfType(type);
        }
    }
}