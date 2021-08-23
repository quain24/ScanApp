using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using System;
using System.Collections.Generic;

namespace ScanApp.Tests
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

        public static List<ApplicationUser> CreateValidListOfUsers()
        {
            return new List<ApplicationUser>
            {
                CreateValidUser(),
                new()
                {
                    ConcurrencyStamp = "6587454",
                    Email = "new_user@altdomain.com",
                    NormalizedEmail = "NEW_USER@ALTDOMAIN.COM",
                    AccessFailedCount = 0,
                    EmailConfirmed = true,
                    Id = "2",
                    LockoutEnabled = true,
                    UserName = "new_user",
                    NormalizedUserName = "NEW_USER",
                    PhoneNumber = "665665665",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false
                }
            };
        }
    }
}