using FluentValidation;
using FluentValidation.Results;
using ScanApp.Application.Common.Interfaces;
using System;

namespace ScanApp.Application.Common.Validators
{
    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator(IUserManager userManager)
        {
            _ = userManager ?? throw new ArgumentNullException(nameof(userManager));

            RuleFor(s => s)
                .CustomAsync(async (pass, context, _) =>
                {
                    var results = await userManager.ValidatePassword(pass).ConfigureAwait(false);
                    foreach (var (code, message) in results)
                    {
                        context.AddFailure(new ValidationFailure(string.Empty, message)
                        {
                            ErrorCode = code
                        });
                    }
                });
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context.InstanceToValidate is not null)
                return true;

            result.Errors.Add(new ValidationFailure(string.Empty, "Provided password was NULL"));
            return false;
        }
    }
}