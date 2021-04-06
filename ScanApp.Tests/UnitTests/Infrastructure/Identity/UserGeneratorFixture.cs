using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using System;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class UserGeneratorFixture
    {
        public static ApplicationUser CreateValidUser()
        {
            var user = new ApplicationUser()
            {
                ConcurrencyStamp = "123456",
                Email = "user@domain.com",
                NormalizedEmail = "USER@DOMAIN.COM",
                AccessFailedCount = 0,
                EmailConfirmed = true,
                Id = "1",
                LockoutEnabled = true,
                UserName = "user",
                NormalizedUserName = "USER",
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            };

            var pwdHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = pwdHasher.HashPassword(user, "password");

            return user;
        }
    }
}