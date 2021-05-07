using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static AuthenticationState BuildAuthState(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }

        private static AuthenticationState NullAuthState() => null;

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_returns_true_if_there_is_a_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var data = claimData[0];
            var testData = new ClaimModel(data.Type, data.Value);

            var result = BuildAuthState(claimData).HasClaim(testData);
            result.Should().BeTrue();
        }

        [Fact]
        public void HasClaim_ClaimModel_throws_arg_null_exc_if_AuthState_is_null()
        {
            var testData = new ClaimModel("type", "value");
            Action act = () => NullAuthState().HasClaim(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_throws_arg_null_exc_if_given_data_is_null(List<Claim> claimData)
        {
            ClaimModel testData = null;
            Action act = () => BuildAuthState(claimData).HasClaim(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_returns_false_if_there_is_no_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var testData = new ClaimModel("data", "value");

            var result = BuildAuthState(claimData).HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_ClaimModel_returns_false_if_Type_of_claim_match_but_value_does_not(List<Claim> claimData)
        {
            var testData = new ClaimModel(claimData[0].Type, "unknown");

            var result = BuildAuthState(claimData).HasClaim(testData);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_returns_true_if_there_is_a_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var data = claimData[0];
            var testData = new ScanApp.Domain.Entities.Claim(data.Type, data.Value);

            var result = BuildAuthState(claimData).HasClaim(testData);
            result.Should().BeTrue();
        }

        [Fact]
        public void HasClaim_Claim_throws_arg_null_exc_if_AuthState_is_null()
        {
            var testData = new ScanApp.Domain.Entities.Claim("type", "value");
            Action act = () => NullAuthState().HasClaim(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_throws_arg_null_exc_if_given_data_is_null(List<Claim> claimData)
        {
            ScanApp.Domain.Entities.Claim testData = null;
            Action act = () => BuildAuthState(claimData).HasClaim(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_returns_false_if_there_is_no_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var testData = new ScanApp.Domain.Entities.Claim("data", "value");
            var result = BuildAuthState(claimData).HasClaim(testData);

            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_Claim_returns_false_if_Type_of_claim_match_but_value_does_not(List<Claim> claimData)
        {
            var testData = new ScanApp.Domain.Entities.Claim(claimData[0].Type, "unknown");
            var result = BuildAuthState(claimData).HasClaim(testData);

            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_true_if_there_is_a_claim_of_given_type(List<Claim> claimData)
        {
            var testData = claimData[0].Type;
            var result = BuildAuthState(claimData).HasClaim(testData);

            result.Should().BeTrue();
        }

        [Fact]
        public void HasClaim_string_throws_arg_null_exc_if_AuthState_is_null()
        {
            var testData = "type";
            Action act = () => NullAuthState().HasClaim(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_throws_arg_null_exc_if_given_data_is_null(List<Claim> claimData)
        {
            string testData = null;
            Action act = () => BuildAuthState(claimData).HasClaim(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_false_if_there_is_no_claim_of_given_type(List<Claim> claimData)
        {
            var testData = "data";
            var result = BuildAuthState(claimData).HasClaim(testData);

            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_true_if_there_is_a_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var type = claimData[0].Type;
            var value = claimData[0].Value;

            var result = BuildAuthState(claimData).HasClaim(type, value);
            result.Should().BeTrue();
        }

        [Fact]
        public void HasClaim_string_type_and_value_throws_arg_null_exc_if_AuthState_is_null()
        {
            var testData = "type";
            Action act = () => NullAuthState().HasClaim(testData, "value");

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_type_and_value_throws_arg_null_exc_if_given_data_is_null(List<Claim> claimData)
        {
            string testData = null;
            Action act = () => BuildAuthState(claimData).HasClaim(testData, "value");

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_type_and_value_throws_arg_null_exc_if_given_value_is_null(List<Claim> claimData)
        {
            Action act = () => BuildAuthState(claimData).HasClaim("type", null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_false_if_there_is_no_claim_of_given_type_and_value(List<Claim> claimData)
        {
            var result = BuildAuthState(claimData).HasClaim("data", "value");
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void HasClaim_string_returns_false_if_Type_of_claim_match_but_value_does_not(List<Claim> claimData)
        {
            var type = claimData[0].Type;
            var result = BuildAuthState(claimData).HasClaim(type, "unknown_value");

            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void GetFirstClaimValue_returns_first_found_value_of_given_claim_type(List<Claim> claimData)
        {
            var type = claimData[0].Type;
            var value = claimData[0].Value;
            var result = BuildAuthState(claimData).GetFirstClaimValue(type);

            result.Should().Be(value);
        }

        [Fact]
        public void GetFirstClaimValue_throws_arg_null_exc_if_AuthState_is_null()
        {
            Action act = () => NullAuthState().GetFirstClaimValue("type");

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void GetFirstClaimValue_throws_arg_null_exc_if_given_data_is_null(List<Claim> claimData)
        {
            string testData = null;
            Action act = () => BuildAuthState(claimData).GetFirstClaimValue(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void GetFirstClaimValue_returns_null_if_no_claim_is_found_with_matching_type(List<Claim> claimData)
        {
            var type = "unknown_claim_type";
            var result = BuildAuthState(claimData).GetFirstClaimValue(type);

            result.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void GetClaimValues_returns_claim_values_for_given_type(List<Claim> claimData)
        {
            var type = claimData[0].Type;
            var value = claimData.Where(v => v.Type == type).Select(v => v.Value);
            var result = BuildAuthState(claimData).GetClaimValues(type);

            result.Should().BeEquivalentTo(value);
        }

        [Fact]
        public void GetClaimValues_throws_arg_null_exc_if_AuthState_is_null()
        {
            Action act = () => NullAuthState().GetClaimValues("type");

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void GetClaimValues_throws_arg_null_exc_if_given_data_is_null(List<Claim> claimData)
        {
            string testData = null;
            Action act = () => BuildAuthState(claimData).GetClaimValues(testData);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(HasClaimData))]
        public void GetClaimValues_returns_empty_collection_if_no_claim_is_found_with_matching_type(List<Claim> claimData)
        {
            var type = "unknown_claim_type";
            var result = BuildAuthState(claimData).GetClaimValues(type);

            result.Should().BeEmpty();
        }
    }
}