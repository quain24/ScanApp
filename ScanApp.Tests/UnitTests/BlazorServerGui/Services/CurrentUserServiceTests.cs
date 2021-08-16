using FluentAssertions;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Exceptions;
using ScanApp.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Services
{
    public class CurrentUserServiceTests : AuthenticationStateProviderFixtures
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new CurrentUserService(ProviderMock.Object);

            subject.Should().BeOfType<CurrentUserService>()
                .And.BeAssignableTo<ICurrentUserService>();
        }

        [Fact]
        public void Will_throw_arg_null_ext_if_missing_AuthenticationStateProvider()
        {
            Action act = () => _ = new CurrentUserService(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Throws_WrongScopeException_if_used_in_new_scope()
        {
            // When calling upon AuthenticationStateProvider.GetAuthenticationStateAsync()
            // InvalidOperationException is thrown if this call has been made in new scope.

            ProviderMock.Setup(m => m.GetAuthenticationStateAsync()).ThrowsAsync(new InvalidOperationException());
            var subject = new CurrentUserService(ProviderMock.Object);

            Func<Task> act = async () => _ = await subject.Name();

            (await act.Should().ThrowAsync<WrongScopeException>())
                .WithInnerException<InvalidOperationException>();
        }

        // Extension methods used in CurrentUserService are tested separately.
    }
}