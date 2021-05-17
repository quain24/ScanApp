using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ScanApp.Tests.UnitTests.BlazorServerGui
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
