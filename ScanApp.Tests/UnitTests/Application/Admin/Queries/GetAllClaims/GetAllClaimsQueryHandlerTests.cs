using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Queries.GetAllClaims;
using ScanApp.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetAllClaims
{
    public class GetAllClaimsQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new GetAllClaimsQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<GetAllClaimsQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new GetAllClaimsQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Throws_if_exception_other_than_OperationCanceledException_happens()
        {
            ContextFactoryMock.Setup(m => m.CreateDbContext()).Throws<Exception>();

            var subject = new GetAllClaimsQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetAllClaimsQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Context_from_factory_is_disposed()
        {
            var claimSourceMock = Array.Empty<Claim>()
                .AsQueryable()
                .BuildMockDbSet();
            ContextMock.Setup(c => c.ClaimsSource).Returns(claimSourceMock.Object);

            var subject = new GetAllClaimsQueryHandler(ContextFactoryMock.Object);

            var _ = await subject.Handle(new GetAllClaimsQuery(), CancellationToken.None);
            try
            {
                ContextMock.Verify(c => c.DisposeAsync(), Times.Once);
            }
            catch (MockException)
            {
                ContextMock.Verify(c => c.Dispose(), Times.Once);
            }
        }

        [Fact]
        public async Task Returns_valid_result_with_list_of_distinct_claims()
        {
            var claims = new ClaimModel[]
            {
                new("type", "value"),
                new("type2", "value2"),
                new("type3", "value3"),
                new("type3", "value4"),
                new("type2", "value2"),
            };

            // Did not use distinct - do not want to depend on ClaimModel equals
            var uniqueClaims = new ClaimModel[]
            {
                new("type", "value"),
                new("type2", "value2"),
                new("type3", "value3"),
                new("type3", "value4"),
            };

            var id = 0;
            var claimSourceMock = claims
                .Select(c => new Claim(id++, c.Type, c.Value))
                .AsQueryable()
                .BuildMockDbSet();
            ContextMock.Setup(c => c.ClaimsSource).Returns(claimSourceMock.Object);

            var subject = new GetAllClaimsQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetAllClaimsQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().OnlyHaveUniqueItems(c => c.Type + c.Value);
            result.Output.Should().BeEquivalentTo(uniqueClaims, opt => opt.ExcludingMissingMembers());
        }

        [Fact]
        public async Task Returns_valid_result_with_empty_list_if_there_is_no_claims()
        {
            var claimSourceMock = Array.Empty<Claim>()
                .AsQueryable()
                .BuildMockDbSet();
            ContextMock.Setup(c => c.ClaimsSource).Returns(claimSourceMock.Object);

            var subject = new GetAllClaimsQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetAllClaimsQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }
    }
}