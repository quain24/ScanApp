using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    public class CreateSparePartsCommandHandlerTests : IContextFactoryMockFixtures
    {
        public ITestOutputHelper Output { get; }

        public CreateSparePartsCommandHandlerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_instance()
        {
            var subject = new CreateSparePartsCommandHandler(Mock.Of<IContextFactory>());

            subject.Should().NotBeNull()
                .And.BeOfType<CreateSparePartsCommandHandler>()
                .And.BeAssignableTo<IRequestHandler<CreateSparePartsCommand, Result>>();
        }

        [Fact]
        public void Throws_ArgNull_exc_if_missing_ContextFactory()
        {
            Action act = () => _ = new CreateSparePartsCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_result_created_when_successful()
        {
            var command = new CreateSparePartsCommand();
            var subject = new CreateSparePartsCommandHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Created);
        }

        [Fact]
        public async Task Context_is_disposed()
        {
            var command = new CreateSparePartsCommand();
            var subject = new CreateSparePartsCommandHandler(ContextFactoryMock.Object);

            await subject.Handle(command, CancellationToken.None);

            ContextMock.Verify(x => x.DisposeAsync(), Times.AtLeastOnce);
        }
    }
}