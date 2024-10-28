using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class DogCreateDtoValidator : AbstractValidator<DogCreateDto>
    {
        public DogCreateDtoValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name can't be longer than 50 characters.");

            RuleFor(d => d.Color)
                .NotEmpty().WithMessage("Color is required.");

            RuleFor(d => d.TailLength)
                .GreaterThan(0).WithMessage("Tail length must be a positive number.");

            RuleFor(d => d.Weight)
                .GreaterThan(0).WithMessage("Weight must be a positive number.");
        }
    }
}
