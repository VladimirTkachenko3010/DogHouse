using Application.DTOs;
using Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Tests
{
    public class DogQueryParametersValidatorTests
    {
        private readonly DogQueryParametersValidator _validator = new();

        [Fact]
        public void Should_HaveError_WhenPageNumberIsZero()
        {
            // Arrange
            var model = new DogQueryParameters { PageNumber = 0 };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(p => p.PageNumber).WithErrorMessage("Page number must be greater than zero.");
        }

        [Fact]
        public void Should_HaveError_WhenPageSizeIsZero()
        {
            // Arrange
            var model = new DogQueryParameters { PageSize = 0 };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(p => p.PageSize).WithErrorMessage("Page size must be greater than zero.");
        }

        [Fact]
        public void Should_HaveError_WhenOrderIsInvalid()
        {
            // Arrange
            var model = new DogQueryParameters { Order = "invalid" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(p => p.Order).WithErrorMessage("Order must be either 'asc' or 'desc'.");
        }

        [Fact]
        public void Should_HaveError_WhenAttributeIsInvalid()
        {
            // Arrange
            var model = new DogQueryParameters { Attribute = "InvalidAttribute" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(p => p.Attribute)
                  .WithErrorMessage("Invalid attribute for sorting. Must be one of: 'Name', 'Color', 'TailLength', 'Weight'");
        }

        [Fact]
        public void Should_NotHaveError_WhenDogQueryParametersIsValid()
        {
            // Arrange
            var model = new DogQueryParameters { PageNumber = 1, PageSize = 10, Order = "asc", Attribute = "Name" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
