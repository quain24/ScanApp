using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using System.Security.Claims;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Services
{
    public abstract class AuthenticationStateProviderFixtures
    {
        public Mock<AuthenticationStateProvider> ProviderMock { get; }
        public Mock<AuthenticationState> AuthStateMock { get; }

        public AuthenticationStateProviderFixtures()
        {
            AuthStateMock = new Mock<AuthenticationState>(new ClaimsPrincipal());
            ProviderMock = new Mock<AuthenticationStateProvider>();
            ProviderMock.Setup(p => p.GetAuthenticationStateAsync()).ReturnsAsync(AuthStateMock.Object);
        }
    }
}