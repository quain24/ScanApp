using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    public class CreateSparePartsCommandHandlerTests
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
    }
}