using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Common.Extensions;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Extensions
{
    public class AuthenticationStateExtensionsTests
    {
        private readonly string _nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        [Fact]
        public void IsItMe_returns_true_if_given_name_is_the_same_as_currently_logged_user_name()
        {
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(m => m.FindFirst(_nameClaimType)).Returns(new Claim(_nameClaimType, "name"));
            var authState = new AuthenticationState(claimsPrincipalMock.Object);

            var result = authState.IsItMe("name");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsItMe_returns_false_if_given_name_is_different_from_currently_logged_user_name()
        {
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(m => m.FindFirst(_nameClaimType)).Returns(new Claim(_nameClaimType, "other_name"));
            var authState = new AuthenticationState(claimsPrincipalMock.Object);

            var result = authState.IsItMe("name");
            result.Should().BeFalse();
        }

        [Fact]
        public void IsItMe_returns_false_if_AuthenticationState_does_not_have_name_claim()
        {
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(m => m.FindFirst(_nameClaimType)).Returns(null as Claim);
            var authState = new AuthenticationState(claimsPrincipalMock.Object);

            var result = authState.IsItMe("name");
            result.Should().BeFalse();
        }

        [Fact]
        public void Name_returns_logged_user_name()
        {
            var identity = new ClaimsIdentity(new[] { new Claim(_nameClaimType, "name") });
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.Name();
            result.Should().Be("name");
        }

        [Fact]
        public void LocationId_returns_id_of_currently_logged_user_location_claim()
        {
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(m => m.FindFirst(Globals.ClaimTypes.Location)).Returns(new Claim(Globals.ClaimTypes.Location, "location_id"));
            var authState = new AuthenticationState(claimsPrincipalMock.Object);

            var result = authState.LocationId();
            result.Should().Be("location_id");
        }

        [Fact]
        public void LocationId_returns_null_if_current_user_does_not_have_location_claim()
        {
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(m => m.FindFirst(Globals.ClaimTypes.Location)).Returns(null as Claim);
            var authState = new AuthenticationState(claimsPrincipalMock.Object);

            var result = authState.LocationId();
            result.Should().BeNull();
        }

        public static TheoryData<List<Claim>> HasClaimData => new()
        {
            new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_a", "value_b"),
                new ("type_b", "value_b"),
                new ("type_b", "value_a"),
                new ("type_b", "value_c")
            }
        };

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_returns_true_if_there_is_a_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var data = claimData[0];
            var testData = new ClaimModel(data.Type, data.Value);
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_returns_false_if_there_is_no_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var testData = new ClaimModel("data", "value");
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_returns_false_if_Type_of_claim_match_but_value_does_not(List<Claim> claimData)
        {
            var testData = new ClaimModel(claimData[0].Type, "unknown");
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_returns_true_if_there_is_a_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var data = claimData[0];
            var testData = new ScanApp.Domain.Entities.Claim(data.Type, data.Value);
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_returns_false_if_there_is_no_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var testData = new ScanApp.Domain.Entities.Claim("data", "value");
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_returns_false_if_Type_of_claim_match_but_value_does_not(List<Claim> claimData)
        {
            var testData = new ScanApp.Domain.Entities.Claim(claimData[0].Type, "unknown");
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_true_if_there_is_a_claim_of_given_type(List<Claim> claimData)
        {
            var data = claimData[0].Type;
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(data);
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_false_if_there_is_no_claim_of_given_type(List<Claim> claimData)
        {
            var testData = "data";
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_true_if_there_is_a_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var type = claimData[0].Type;
            var value = claimData[0].Value;
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(type, value);
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_false_if_there_is_no_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim("data", "value");
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_false_if_Type_of_claim_match_but_value_does_not(List<Claim> claimData)
        {
            var type = claimData[0].Type;
            var identity = new ClaimsIdentity(claimData);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var result = authState.HasClaim(type, "unknown_value");
            result.Should().BeFalse();
        }
    }
}