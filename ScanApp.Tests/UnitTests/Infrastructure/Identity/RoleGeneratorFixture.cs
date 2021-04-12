using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class RoleGeneratorFixture
    {
        public static List<IdentityRole> CreateRoleCollection()
        {
            return new()
            {
                new("role_a"),
                new("role_b")
            };
        }
    }
}