using FluentValidation;
using FluentValidation.Results;
using System.Linq;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Extensions
{
    public static class AbstractValidatorExtensionsFixtures
    {
        internal class testValidatorString : AbstractValidator<string>
        {
            public testValidatorString()
            {
                RuleFor(x => x).NotEmpty().MaximumLength(3);
            }

            protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
            {
                if (context.InstanceToValidate is not null) return true;

                var name = CreateDescriptor().Rules?.First()?.PropertyName ?? "Field";
                result.Errors.Add(new ValidationFailure(name, $"{name} cannot be empty."));
                return false;
            }
        }

        internal class testValidatorStringNoPrevalidate : AbstractValidator<string>
        {
            public testValidatorStringNoPrevalidate()
            {
                RuleFor(x => x).NotEmpty().MaximumLength(3);
            }
        }

        internal class testValidatorInt : AbstractValidator<int>
        {
            public testValidatorInt()
            {
                RuleFor(x => x).LessThan(10);
            }
        }
    }
}