using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Application.Common.Entities;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public static class UserManagerFixture
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls, IdentityResult deleteResult = null, IdentityResult createResult = null, IdentityResult updateResult = null, TUser findByNameResult = null) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(deleteResult ?? IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(createResult ?? IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(updateResult ?? IdentityResult.Success);
            mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(findByNameResult);
            return mgr;
        }
    }
}