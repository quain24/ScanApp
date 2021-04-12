using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class RoleManagerServiceTests
    {
        [Fact]
        public void Will_create_RoleManagerService()
        {
            var roleManagerMock = RoleManagerFixture.MockRoleManager();

            var sut = new RoleManagerService(roleManagerMock.Object);

            sut.Should().NotBeNull()
                .And.Subject
                .Should().BeOfType<RoleManagerService>()
                .And
                .BeAssignableTo<IRoleManager>();
        }

        [Fact]
        public void Will_throw_if_no_RoleManager_was_given()
        {
            Action act = () => new RoleManagerService(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetAllRoles_returns_all_existing_roles()
        {
            var roles = RoleGeneratorFixture.CreateRoleCollection();
            var roleManagerMock = RoleManagerFixture.MockRoleManager(roles);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.GetAllRoles();

            result.Conclusion.Should().BeTrue();
            result.Output.Should().HaveCount(roles.Count)
                .And.Subject.Should().BeEquivalentTo(roles, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task AddNewRole_will_add_new_role_to_collection_and_return_valid_result()
        {
            var roles = RoleGeneratorFixture.CreateRoleCollection();
            var comparedRoles = RoleGeneratorFixture.CreateRoleCollection();
            var roleManagerMock = RoleManagerFixture.MockRoleManager(roles);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.AddNewRole("new_role");

            result.Conclusion.Should().BeTrue();
            roles.Should().HaveCount(comparedRoles.Count + 1);
            roles.Should().ContainSingle(r => r.Name.Equals("new_role"));
            using var sc = new AssertionScope();
            comparedRoles.ForEach(r => roles.Should().ContainEquivalentOf(r, opt =>
            {
                opt.Excluding(e => e.ConcurrencyStamp);
                opt.Excluding(e => e.Id);
                return opt;
            }));
        }

        [Fact]
        public async Task RemoveRole_will_delete_role()
        {
            var roleToDelete = RoleGeneratorFixture.CreateRoleCollection()[0];
            var roleManagerMock = RoleManagerFixture.MockRoleManager(findByNameResult: roleToDelete);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.RemoveRole(roleToDelete.Name);

            result.Conclusion.Should().BeTrue();
            roleManagerMock.Verify(r => r.DeleteAsync(It.Is<IdentityRole>(s => s.Id.Equals(roleToDelete.Id))), Times.Once);
        }

        [Fact]
        public async Task RemoveRole_will_return_invalid_result_of_not_found_if_given_role_does_not_exist()
        {
            var roleToDelete = new IdentityRole("role_to_delete");
            var roleManagerMock = RoleManagerFixture.MockRoleManager();

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.RemoveRole(roleToDelete.Name);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            roleManagerMock.Verify(r => r.DeleteAsync(It.Is<IdentityRole>(s => s.Id.Equals(roleToDelete.Id))), Times.Never);
        }

        [Fact]
        public async Task EditRoleName_will_change_role_name()
        {
            var roles = RoleGeneratorFixture.CreateRoleCollection();
            var roleToUpdate = roles[0];
            var oldName = roleToUpdate.Name;
            var newName = "updated_name";

            // will later compare to original collection excluding modified role
            var comparedRoles = RoleGeneratorFixture.CreateRoleCollection();
            comparedRoles.RemoveAt(0);
            var roleManagerMock = RoleManagerFixture.MockRoleManager(findByNameResult: roleToUpdate);
            roleManagerMock.Setup(m => m.SetRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<IdentityRole, string>((i, n) => i.Name = n);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.EditRoleName(roleToUpdate.Name, newName);

            result.Conclusion.Should().BeTrue();
            // adding one for role removed in 'arrange' step
            roles.Should().HaveCount(comparedRoles.Count + 1);
            roles.Should().ContainSingle(r => r.Name.Equals(newName));
            roles.Should().NotContain(r => r.Name.Equals(oldName));
            using var sc = new AssertionScope();
            comparedRoles.ForEach(r => roles.Should().ContainEquivalentOf(r, opt =>
            {
                opt.Excluding(e => e.ConcurrencyStamp);
                opt.Excluding(e => e.Id);
                return opt;
            }));
            roleManagerMock.Verify(m => m.SetRoleNameAsync(It.Is<IdentityRole>(r => r.Id.Equals(roleToUpdate.Id)), It.Is<string>(n => n.Equals(newName))), Times.Once);
        }

        [Fact]
        public async Task EditRoleName_will_return_invalid_not_found_result_if_no_role_with_given_name_exists()
        {
            var roleManagerMock = RoleManagerFixture.MockRoleManager();

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.EditRoleName("role_name", "updated_name");

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            roleManagerMock.Verify(m => m.SetRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetAllClaimsFromRole_returns_all_claims_of_given_role()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.GetAllClaimsFromRole(role.Name);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(claims, opt => opt.ExcludingMissingMembers());
            roleManagerMock.Verify(r => r.GetClaimsAsync(It.Is<IdentityRole>(i => i.Id.Equals(role.Id))), Times.Once);
        }

        [Fact]
        public async Task GetAllClaimsFromRole_will_return_invalid_not_found_result_if_no_role_with_given_name_exists()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var roleManagerMock = RoleManagerFixture.MockRoleManager();

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.GetAllClaimsFromRole(role.Name);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            roleManagerMock.Verify(r => r.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Fact]
        public async Task AddClaimToRole_will_add_new_claim_to_role()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);
            roleManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<IdentityRole, Claim>((_, c) => claims.Add(c));
            var claimToAdd = new ClaimModel("new_type", "new_value");

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.AddClaimToRole(role.Name, claimToAdd);

            result.Conclusion.Should().BeTrue();
            roleManagerMock.Verify(x => x.AddClaimAsync
            (
                It.Is<IdentityRole>(r => r.Id.Equals(role.Id)),
                It.Is<Claim>(c => c.Type.Equals(claimToAdd.Type) && c.Value.Equals(claimToAdd.Value))),
                Times.Once
            );
            claims.Should().ContainSingle(c => c.Type == claimToAdd.Type && c.Value == claimToAdd.Value);
        }

        [Fact]
        public async Task AddClaimToRole_wont_add_claim_if_role_has_same_one_already()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);
            var claimToAdd = new ClaimModel(claims[0].Type, claims[0].Value);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.AddClaimToRole(role.Name, claimToAdd);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Duplicated);
            roleManagerMock.Verify(x => x.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task AddClaimToRole_will_return_invalid_not_found_result_if_no_role_with_given_name_exists()
        {
            var roleManagerMock = RoleManagerFixture.MockRoleManager();

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.AddClaimToRole("not_found_role", new ClaimModel("any_type", "any_value"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            roleManagerMock.Verify(x => x.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task RemoveClaimFromRole_will_remove_given_claim_from_given_role()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var roleManagerMock = RoleManagerFixture.MockRoleManager(findByNameResult: role);
            roleManagerMock.Setup(x => x.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);
            var claimToDelete = new ClaimModel("type", "value");

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.RemoveClaimFromRole(role.Name, claimToDelete.Type, claimToDelete.Value);

            result.Conclusion.Should().BeTrue();
            roleManagerMock.Verify(x => x.RemoveClaimAsync
            (
                It.Is<IdentityRole>(r => r.Id.Equals(role.Id)),
                It.Is<Claim>(c => c.Type.Equals(claimToDelete.Type) && c.Value.Equals(claimToDelete.Value))),
                Times.Once
            );
        }

        [Fact]
        public async Task RemoveClaimFromRole_will_return_invalid_not_found_result_if_no_role_with_given_name_exists()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var roleManagerMock = RoleManagerFixture.MockRoleManager();
            var claimToDelete = new ClaimModel("type", "value");

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.RemoveClaimFromRole(role.Name, claimToDelete.Type, claimToDelete.Value);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            roleManagerMock.Verify(x => x.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task HasClaim_will_return_valid_true_result_if_role_has_claim()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);
            var claimToFind = claims[0];

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.HasClaim(role.Name, claimToFind.Type, claimToFind.Value);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeTrue();
            roleManagerMock.Verify(x => x.GetClaimsAsync(It.Is<IdentityRole>(r => r.Id.Equals(role.Id))), Times.Once);
        }

        [Fact]
        public async Task HasClaim_will_return_valid_true_result_if_role_has_any_claim_with_given_type_if_only_type_is_given()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);
            var claimToFind = claims[0];

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.HasClaim(role.Name, claimToFind.Type);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeTrue();
            roleManagerMock.Verify(x => x.GetClaimsAsync(It.Is<IdentityRole>(r => r.Id.Equals(role.Id))), Times.Once);
        }

        [Fact]
        public async Task HasClaim_will_return_valid_false_result_if_role_does_not_have_given_claim()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.HasClaim(role.Name, "some_type", "some_value");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeFalse();
            roleManagerMock.Verify(x => x.GetClaimsAsync(It.Is<IdentityRole>(r => r.Id.Equals(role.Id))), Times.Once);
        }

        [Fact]
        public async Task HasClaim_will_return_valid_false_result_if_role_does_not_have_given_claim_given_only_claim_type()
        {
            var role = RoleGeneratorFixture.CreateRoleCollection()[0];
            var claims = new List<Claim>
            {
                new ("type_a", "value_a"),
                new ("type_b", "value_b")
            };
            var roleManagerMock = RoleManagerFixture.MockRoleManager(getClaimsAsync: claims, findByNameResult: role);

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.HasClaim(role.Name, "some_type");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeFalse();
            roleManagerMock.Verify(x => x.GetClaimsAsync(It.Is<IdentityRole>(r => r.Id.Equals(role.Id))), Times.Once);
        }

        [Fact]
        public async Task HasClaim_will_return_invalid_not_found_result_if_no_role_with_given_name_exists()
        {
            var roleManagerMock = RoleManagerFixture.MockRoleManager();
            var claimToCheck = new ClaimModel("type", "value");

            var sut = new RoleManagerService(roleManagerMock.Object);

            var result = await sut.HasClaim("not_existing_role", claimToCheck.Type, claimToCheck.Value);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            roleManagerMock.Verify(x => x.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
        }
    }
}