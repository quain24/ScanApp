using FluentValidation;

namespace ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
    internal class SparePartStoragePlacesByLocationQueryValidator : AbstractValidator<SparePartStoragePlacesByLocationQuery>
    {
        public SparePartStoragePlacesByLocationQueryValidator()
        {
            RuleFor(e => e.LocationId)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be null or empty or contain only whitespaces");
        }
    }
}