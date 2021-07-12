using FluentValidation;

namespace ScanApp.Application.HesHub.Depots.Commands
{
    public class GateModelValidator : AbstractValidator<GateModel>
    {
        public GateModelValidator()
        {
            RuleFor(x => x.Version)
                .NotNull();
        }
    }
}