using Application.DTOs;
using Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Tests
{
    public class DogCreateDtoValidatorTests
    {
        private readonly DogCreateDtoValidator _validator = new();

        [Fact]
        public void Should_HaveError_WhenNameIsEmpty()
        {
            // Arrange
            var model = new DogCreateDto { Name = "" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(d => d.Name).WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_HaveError_WhenNameIsTooLong()
        {
            // Arrange
            var model = new DogCreateDto { Name = new string('a', 51) };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(d => d.Name).WithErrorMessage("Name can't be longer than 50 characters.");
        }

        [Fact]
        public void Should_HaveError_WhenTailLengthIsNegative()
        {
            // Arrange
            var model = new DogCreateDto { TailLength = -1 };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(d => d.TailLength).WithErrorMessage("Tail length must be a positive number.");
        }

        [Fact]
        public void Should_NotHaveError_WhenDogCreateDtoIsValid()
        {
            // Arrange
            var model = new DogCreateDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
