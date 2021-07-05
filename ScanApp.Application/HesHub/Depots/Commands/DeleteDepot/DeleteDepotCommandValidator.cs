using FluentValidation;

namespace ScanApp.Application.HesHub.Depots.Commands.DeleteDepot
{
    public class DeleteDepotCommandValidator : AbstractValidator<DeleteDepotCommand>
    {
        public DeleteDepotCommandValidator()
        {
            RuleFor(e => e.Version)
                .NotNull().Must(v => v?.IsEmpty is false).WithMessage("Version must not be empty when deleting entity.");
        }
    }
}