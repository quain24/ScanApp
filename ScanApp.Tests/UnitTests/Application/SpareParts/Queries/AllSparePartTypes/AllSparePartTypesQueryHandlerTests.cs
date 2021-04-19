using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Queries.AllSparePartTypes;
using ScanApp.Domain.Entities;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Queries.AllSparePartTypes
{
    public class AllSparePartTypesQueryHandlerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var subject = new AllSparePartTypesQueryHandler(contextFactoryMock.Object);

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
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            contextFactoryMock.Setup(c => c.CreateDbContext()).Returns(contextMock.Object);
            var data = new List<SparePartType>
            {
                new("part_a"),
                new("part_b"),
                new("part_c")
            };
            var dataMock = data.AsQueryable().BuildMockDbSet();
            contextMock.SetupGet(c => c.SparePartTypes).Returns(dataMock.Object);

            var subject = new AllSparePartTypesQueryHandler(contextFactoryMock.Object);
            var result = await subject.Handle(new AllSparePartTypesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(data);
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            contextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new AllSparePartTypesQueryHandler(contextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new AllSparePartTypesQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_invalid_result_of_cancelled_if_cancellation_occurred()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            contextFactoryMock.Setup(c => c.CreateDbContext()).Returns(contextMock.Object);
            var token = new CancellationTokenSource(0).Token;
            contextMock.SetupGet(c => c.SparePartTypes).Throws<OperationCanceledException>();

            var subject = new AllSparePartTypesQueryHandler(contextFactoryMock.Object);
            var result = await subject.Handle(new AllSparePartTypesQuery(), token);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Cancelled);
            result.ErrorDescription.Exception.Should().BeOfType<OperationCanceledException>();
        }
    }
}