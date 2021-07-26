using System.Linq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands;
using ScanApp.Domain.ValueObjects;
using ScanApp.Tests.TestExtensions;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands
{
    public class GateModelValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new GateModelValidator();

            subject.Should().BeOfType<GateModelValidator>()
                .And.BeAssignableTo<AbstractValidator<GateModel>>();
        }

        [Fact]
        public void Validates_version()
        {
            var subject = new GateModelValidator();
            var validators = subject.ExtractPropertyValidators();

            validators.Should().HaveCount(1);
            validators.Should().ContainKey(nameof(GateModel.Version))
                .WhichValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeOfType<NotNullValidator<GateModel, Version>>();
        }
    }
}