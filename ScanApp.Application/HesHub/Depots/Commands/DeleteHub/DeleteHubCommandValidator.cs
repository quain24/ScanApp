using FluentValidation;

namespace ScanApp.Application.HesHub.Depots.Commands.DeleteHub
{
    public class DeleteHubCommandValidator : AbstractValidator<DeleteHubCommand>
    {
        public DeleteHubCommandValidator()
        {
            RuleFor(e => e.Version)
                .NotNull().Must(v => v?.IsEmpty is false).WithMessage("Version must not be empty when deleting entity.");
        }
    }
}