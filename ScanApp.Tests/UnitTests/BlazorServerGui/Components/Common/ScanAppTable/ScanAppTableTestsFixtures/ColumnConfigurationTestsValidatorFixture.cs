using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.ScanAppTableTestsFixtures
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
