using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public static class RoleManagerFixture
    {
        public static Mock<RoleManager<IdentityRole>> MockRoleManager(List<IdentityRole> ls = null, IdentityResult deleteResult = null, IdentityResult createResult = null, IdentityResult updateResult = null, IdentityRole findByNameResult = null, List<Claim> getClaimsAsync = null)
        {
            ls ??= new List<IdentityRole>(0);
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mgr = new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
            var queryableList = ls.AsQueryable().BuildMock();
            mgr.SetupGet(p => p.Roles).Returns(queryableList.Object);

            mgr.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(deleteResult ?? IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(createResult ?? IdentityResult.Success).Callback<IdentityRole>(role => ls.Add(role));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(updateResult ?? IdentityResult.Success);
            mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(findByNameResult);
            mgr.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(getClaimsAsync ?? new List<Claim>(0));
            return mgr;
        }
    }
}