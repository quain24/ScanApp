using FluentValidation;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.ScanAppTableTestsFixtures
{
    public class ColumnConfigTestsValidatorFixture : AbstractValidator<int>
    {
        public ColumnConfigTestsValidatorFixture()
        {
            RuleFor(x => x)
                .GreaterThanOrEqualTo(0);
        }
    }
}
