using FluentValidation;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.ScanAppTableTestsFixtures
{
    public class ColumnConfigurationTestsValidatorFixture : AbstractValidator<int>
    {
        public ColumnConfigurationTestsValidatorFixture()
        {
            RuleFor(x => x)
                .GreaterThanOrEqualTo(0);
        }
    }
}
