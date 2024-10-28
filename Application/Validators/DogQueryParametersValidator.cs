using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class DogQueryParametersValidator : AbstractValidator<DogQueryParameters>
    {
        public DogQueryParametersValidator()
        {
            RuleFor(p => p.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(p => p.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than zero.");

            RuleFor(p => p.Order)
                .Must(order => order == "asc" || order == "desc")
                .WithMessage("Order must be either 'asc' or 'desc'.");

            RuleFor(p => p.Attribute)
                .Must(attribute => new[] { "Name", "Color", "TailLength", "Weight" }.Contains(attribute))
                .WithMessage("Invalid attribute for sorting. Must be one of: 'Name', 'Color', 'TailLength', 'Weight'");
        }
    }
}
