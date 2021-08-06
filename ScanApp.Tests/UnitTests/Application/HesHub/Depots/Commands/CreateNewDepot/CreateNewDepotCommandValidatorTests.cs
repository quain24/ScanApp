using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot;
using ScanApp.Tests.TestExtensions;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.CreateNewDepot
{
    public class CreateNewDepotCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new CreateNewDepotCommandValidator();

            subject.Should().NotBeNull()
                .And.BeOfType<CreateNewDepotCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<CreateNewDepotCommand>>();
        }

        [Fact]
        public void Will_use_depot_model_validator()
        {
            var subject = new CreateNewDepotCommandValidator();

            var validators = subject.ExtractPropertyValidators();

            validators.Should().HaveCount(1)
                .And.Subject.First().Value.First().Should().BeOfType<ChildValidatorAdaptor<CreateNewDepotCommand, DepotModel>>();
        }

        [Fact]
        public void Null_model_is_invalid()
        {
            var subject = new CreateNewDepotCommandValidator();

            var result = subject.Validate(null as CreateNewDepotCommand);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}