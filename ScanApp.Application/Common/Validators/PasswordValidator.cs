using FluentValidation;
using FluentValidation.Results;
using ScanApp.Application.Common.Interfaces;
using System;

namespace ScanApp.Application.Common.Validators
{
    /// <summary>
    /// Provides validation for password inputs - checks them against all rules set for Identity Services (part of ASP Core).
    /// </summary>
    public class PasswordValidator : AbstractValidator<string>
    {
        /// <summary>
        /// Creates new instance of <see cref="PasswordValidator"/>.
        /// </summary>
        /// <param name="userManager">User manager used to gain access to Identity service rules.</param>
        public PasswordValidator(IUserManager userManager)
        {
            _ = userManager ?? throw new ArgumentNullException(nameof(userManager));

            RuleFor(s => s)
                .CustomAsync(async (pass, context, _) =>
                {
                    var results = await userManager.ValidatePassword(pass).ConfigureAwait(false);
                    foreach (var (code, message) in results)
                    {
                        context.AddFailure(new ValidationFailure(context.PropertyName, message)
                        {
                            ErrorCode = code,
                            AttemptedValue = pass
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