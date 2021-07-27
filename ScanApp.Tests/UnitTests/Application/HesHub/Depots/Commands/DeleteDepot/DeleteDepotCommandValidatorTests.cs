using FluentAssertions;
using FluentValidation;
using ScanApp.Application.HesHub.Depots.Commands.DeleteDepot;
using ScanApp.Tests.TestExtensions;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.DeleteDepot
{
    public class DeleteDepotCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new DeleteDepotCommandValidator();

            subject.Should().BeOfType<DeleteDepotCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<DeleteDepotCommand>>();
        }

        [Fact]
        public void Validates_version_property()
        {
            var subject = new DeleteDepotCommandValidator();

            var validators = subject.ExtractPropertyValidators();

            validators.Should().HaveCount(1)
                .And.Subject.ContainsKey(nameof(DeleteDepotCommand.Version));
        }
    }
}