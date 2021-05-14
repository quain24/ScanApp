using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
    /// <summary>
    /// Provides validation for <see cref="SparePartStoragePlacesByLocationQuery"/>.
    /// </summary>
    public class SparePartStoragePlacesByLocationQueryValidator : AbstractValidator<SparePartStoragePlacesByLocationQuery>
    {
        /// <summary>
        /// Creates new instance of <see cref="SparePartStoragePlacesByLocationQueryValidator"/>.
        /// </summary>
        public SparePartStoragePlacesByLocationQueryValidator()
        {
            RuleFor(e => e.LocationId)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be null or empty or contain only whitespaces");
        }

        protected override bool PreValidate(ValidationContext<SparePartStoragePlacesByLocationQuery> context, ValidationResult result)
        {
            if (context.InstanceToValidate is not null)
                return true;

            result.Errors.Add(new ValidationFailure(string.Empty, $"Provided {nameof(SparePartStoragePlacesByLocationQuery)} instance was NULL"));
            return false;
        }
    }
}