using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class DogCreateDtoValidator : AbstractValidator<DogCreateDto>
    {
        public DogCreateDtoValidator()
        {
            RuleFor(d => d.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(d => d.TailLength).GreaterThan(0).WithMessage("Tail length must be positive.");
            RuleFor(d => d.Weight).GreaterThan(0).WithMessage("Weight must be positive.");
        }
    }
}
