using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands;
using ScanApp.Domain.ValueObjects;
using ScanApp.Tests.TestExtensions;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands
{
    public class TrailerTypeModelValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new TrailerTypeModelValidator();

            subject.Should().BeOfType<TrailerTypeModelValidator>()
                .And.BeAssignableTo<AbstractValidator<TrailerTypeModel>>();
        }

        [Fact]
        public void Validates_version()
        {
            var subject = new TrailerTypeModelValidator();
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(TrailerTypeModel.Version))
                .WhoseValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeOfType<NotNullValidator<TrailerTypeModel, Version>>();
        }

        [Fact]
        public void Validates_name()
        {
            var subject = new TrailerTypeModelValidator();
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(TrailerTypeModel.Name))
                .WhoseValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeOfType<NotEmptyValidator<TrailerTypeModel, string>>();
        }
    }
}