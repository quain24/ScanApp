using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands;
using ScanApp.Application.HesHub.Depots.Commands.EditDepot;
using ScanApp.Tests.TestExtensions;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.EditDepot
{
    public class EditDepotCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new EditDepotCommandValidator();

            subject.Should().BeOfType<EditDepotCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<EditDepotCommand>>();
        }

        [Fact]
        public void Validator_checks_both_models_using_DepotModelValidator()
        {
            var subject = new EditDepotCommandValidator();

            var validators = subject.ExtractPropertyValidators();

            using var context = new AssertionScope();
            validators.Should().HaveCount(2);
            validators.Should().ContainKeys(nameof(EditDepotCommand.EditedModel), nameof(EditDepotCommand.OriginalModel));
            validators.Values.Select(x => x.FirstOrDefault()).Should().AllBeAssignableTo<ChildValidatorAdaptor<EditDepotCommand, DepotModel>>();
            validators.Values.Select(x => x.First()).ToList()
                .ForEach(x => ((ChildValidatorAdaptor<EditDepotCommand, DepotModel>)x).ValidatorType.Should().Be<DepotModelValidator>());
        }
    }
}