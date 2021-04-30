using FluentAssertions;
using FluentValidation;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.Common.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Common.Validators
{
    public class PasswordValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new PasswordValidator(userManagerMock.Object);

            subject.Should().BeOfType<PasswordValidator>()
                .And.BeAssignableTo<AbstractValidator<string>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_IUserManager_is_missing()
        {
            Action act = () => _ = new PasswordValidator(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_use_UserManager_validate_password_method()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(m => m.ValidatePassword("password")).ReturnsAsync(new List<(string, string)>());
            var subject = new PasswordValidator(userManagerMock.Object);

            await subject.ValidateAsync("password");

            userManagerMock.Verify(m => m.ValidatePassword("password"), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Will_invalidate_when_password_is_null_using_prevalidate()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new PasswordValidator(userManagerMock.Object);

            var result = await subject.ValidateAsync(null as string);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            userManagerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Will_validate_proper_password()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(m => m.ValidatePassword("password")).ReturnsAsync(new List<(string, string)>());
            var subject = new PasswordValidator(userManagerMock.Object);

            var result = await subject.ValidateAsync("password");

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Will_invalidate_bad_password_and_get_error_results()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(m => m.ValidatePassword("password")).ReturnsAsync(new List<(string, string)> { ("code", "message") });
            var subject = new PasswordValidator(userManagerMock.Object);

            var result = await subject.ValidateAsync("password");

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().ErrorCode.Should().Be("code");
            result.Errors.First().ErrorMessage.Should().Be("message");
            result.Errors.First().AttemptedValue.Should().Be("password");
        }
    }
}