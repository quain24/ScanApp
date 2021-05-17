using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ScanApp.Common.Validators
{
    public class PositiveNumbersOnlyValidator : AbstractValidator<int?>
    {
        public PositiveNumbersOnlyValidator()
        {
            RuleFor(x => x)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Negative numbers are not allowed.");
        }
    }
}
