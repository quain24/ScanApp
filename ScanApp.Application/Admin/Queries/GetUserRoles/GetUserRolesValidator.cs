using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Queries.GetUserRoles
{
    public class GetUserRolesValidator : AbstractValidator<GetUserRolesQuery>
    {
        public GetUserRolesValidator()
        {
            RuleFor(c => c.UserName)
                .NotEmpty()
                .SetValidator(new IdentityNamingValidator());
        }
    }
}