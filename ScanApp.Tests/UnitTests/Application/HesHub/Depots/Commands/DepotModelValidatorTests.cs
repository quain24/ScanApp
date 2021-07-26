using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentValidation;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands;
using ScanApp.Tests.TestExtensions;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands
{
    public class DepotModelValidatorTests
    {
        private ITestOutputHelper Output { get; }

        public DepotModelValidatorTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_instance()
        {
            var subject = new DepotModelValidator();

            subject.Should().NotBeNull()
                .And.BeOfType<DepotModelValidator>()
                .And.BeAssignableTo<AbstractValidator<DepotModel>>();
        }

        [Fact]
        public void All_properties_should_be_validated_excluding_id()
        {
            var subject = new DepotModelValidator();
            var validators = subject.ExtractPropertyValidators();
            var properties = typeof(DepotModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name != nameof(DepotModel.Id))
                .Select(x => x.Name);

            properties.Should().BeEquivalentTo(validators.Keys);
        }

        [Fact]
        public void Null_model_is_invalid()
        {
            var subject = new DepotModelValidator();

            var result = subject.Validate(null as DepotModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}