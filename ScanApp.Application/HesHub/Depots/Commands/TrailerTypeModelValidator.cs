using FluentValidation;

namespace ScanApp.Application.HesHub.Depots.Commands
{
    public class TrailerTypeModelValidator : AbstractValidator<TrailerTypeModel>
    {
        public TrailerTypeModelValidator()
        {
            RuleFor(x => x.Version)
                .NotNull();

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}