using FluentValidation;

namespace ScanApp.Application.SpareParts.Commands.CreateSpareParts
{
    /// <summary>
    /// Provides validation for <see cref="CreateSparePartsCommand"/>.
    /// </summary>
    public class CreateSparePartsCommandValidator : AbstractValidator<CreateSparePartsCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="CreateSparePartsCommandValidator"/>.
        /// </summary>
        public CreateSparePartsCommandValidator()
        {
            RuleFor(c => c.SpareParts)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Instead of spare parts collection a null was passed inside command.")
                .NotEmpty()
                .WithMessage("There are no spare parts to be added to database, command is invalid");

            RuleForEach(c => c.SpareParts)
                .NotNull()
                .WithMessage("One of passed spare parts is null, aborting!")
                .ChildRules(sparePart =>
                {
                    sparePart.RuleFor(s => s.Amount)
                        .InclusiveBetween(1, 1000)
                        .WithMessage("Spare part quantity must be between 1 and 1000");
                    sparePart.RuleFor(s => s.Name)
                        .NotEmpty()
                        .WithMessage("one of spare parts does not have a Name");
                    sparePart.RuleFor(s => s.SourceArticleId)
                        .NotEmpty()
                        .WithMessage("one of spare parts does not have a Source article ID");
                    sparePart.RuleFor(s => s.SparePartStoragePlaceId)
                        .NotEmpty()
                        .WithMessage("one of spare parts does not have a Storage place ID");
                });
        }
    }
}