using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ScanApp.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Extensions
{
    public class AbstractValidatorExtensionsTests
    {
        [Fact]
        public void ToMudFormFieldValidator_throws_arg_null_exc_if_called_on_null()
        {
            Action act = () => (null as AbstractValidator<int>).ToMudFormFieldValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToAsyncMudFormFieldValidator_throws_arg_null_exc_if_called_on_null()
        {
            Action act = () => (null as AbstractValidator<int>).ToAsyncMudFormFieldValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToMudFormFieldValidator_creates_proper_delegate()
        {
            var validatorMock = new Mock<AbstractValidator<string>>();
            validatorMock.Setup(v => v.Validate(It.Is<ValidationContext<string>>(x =>
                x.InstanceToValidate.Equals("ok", StringComparison.OrdinalIgnoreCase))))
                .Returns(new ValidationResult());
            validatorMock.Setup(v => v.Validate(It.Is<ValidationContext<string>>(x =>
                !x.InstanceToValidate.Equals("ok", StringComparison.OrdinalIgnoreCase))))
                .Returns(new ValidationResult(new[] { new ValidationFailure("property_name", "error") }));

            var subject = validatorMock.Object.ToMudFormFieldValidator();

            subject("ok").Should().BeEmpty();
            subject("invalid").Should().HaveCount(1)
                .And.Subject.First().Should().Be("error");
        }

        [Fact]
        public async Task ToAsyncMudFormFieldValidator_creates_proper_delegate()
        {
            var validatorMock = new Mock<AbstractValidator<string>>();
            validatorMock.Setup(v => v.ValidateAsync(It.Is<ValidationContext<string>>(x =>
                    x.InstanceToValidate.Equals("ok", StringComparison.OrdinalIgnoreCase)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            validatorMock.Setup(v => v.ValidateAsync(It.Is<ValidationContext<string>>(x =>
                    !x.InstanceToValidate.Equals("ok", StringComparison.OrdinalIgnoreCase)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("property_name", "error") }));

            var subject = validatorMock.Object.ToAsyncMudFormFieldValidator();

            (await subject("ok")).Should().BeEmpty();
            (await subject("invalid")).Should().HaveCount(1)
                .And.Subject.First().Should().Be("error");
        }
    }
}