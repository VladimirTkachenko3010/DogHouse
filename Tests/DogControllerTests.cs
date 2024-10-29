using Application.DTOs;
using Application.Interfaces;
using Application.Utilities;
using Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests
{
    public class DogControllerTests
    {
        private readonly Mock<IDogService> _dogServiceMock;
        private readonly DogController _controller;

        public DogControllerTests()
        {
            _dogServiceMock = new Mock<IDogService>();
            _controller = new DogController(_dogServiceMock.Object);

            // Setting up ControllerContext to simulate ExceptionFilter
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void Ping_ShouldReturnOkWithVersion()
        {
            // Act
            var result = _controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Dogshouseservice.Version1.0.1", okResult.Value);
        }

        [Fact]
        public async Task GetDogs_WhenParametersAreValid_ShouldReturnOkWithDogs()
        {
            // Arrange
            var parameters = new DogQueryParameters { PageSize = 10, PageNumber = 1 };
            var dogs = new List<DogDto> { new DogDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 15 } };

            _dogServiceMock.Setup(service => service.GetDogsAsync(parameters)).ReturnsAsync(dogs);

            // Act
            var result = await _controller.GetDogs(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDogs = Assert.IsType<List<DogDto>>(okResult.Value);
            Assert.Single(returnedDogs);  // Assert that we have only one dog returned
        }

        [Fact]
        public async Task GetDogs_WhenParametersAreInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidParameters = new DogQueryParameters { PageSize = 0, PageNumber = 1 }; // Invalid PageSize

            // Act
            var result = await _controller.GetDogs(invalidParameters);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateDog_WhenModelStateIsInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var dogDto = new DogCreateDto { Color = "Brown", TailLength = 10, Weight = 15 };

            // Act
            var result = await _controller.CreateDog(dogDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateDog_WhenDogServiceFails_ShouldReturnBadRequest()
        {
            // Arrange
            var dogDto = new DogCreateDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 15 };
            var result = Result.Failure("An error occurred"); // Example of error result

            _dogServiceMock.Setup(service => service.CreateDogAsync(dogDto)).ReturnsAsync(result);

            // Act
            var response = await _controller.CreateDog(dogDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);

            // We check that there is a list of errors inside Value
            var errors = Assert.IsType<List<string>>(badRequestResult.Value);
            Assert.Contains("An error occurred", errors);
        }

        [Fact]
        public async Task CreateDog_WhenDogServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var dogDto = new DogCreateDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 15 };
            _dogServiceMock.Setup(service => service.CreateDogAsync(dogDto)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var response = await _controller.CreateDog(dogDto);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("An unexpected error occurred.", internalServerErrorResult.Value);
        }

        [Fact]
        public async Task GetDogs_WhenDogServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var parameters = new DogQueryParameters();
            _dogServiceMock.Setup(service => service.GetDogsAsync(parameters)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var response = await _controller.GetDogs(parameters);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("An unexpected error occurred.", internalServerErrorResult.Value);
        }

        [Fact]
        public async Task CreateDog_WhenSuccess_ShouldReturnCreated()
        {
            // Arrange
            var dogDto = new DogCreateDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 15 };
            var result = Result.Success(1); // Assuming 1 is the new ID

            _dogServiceMock.Setup(service => service.CreateDogAsync(dogDto)).ReturnsAsync(result);

            // Act
            var response = await _controller.CreateDog(dogDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(response);
            Assert.Equal(nameof(DogController.GetDogs), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
        }
    }
}
