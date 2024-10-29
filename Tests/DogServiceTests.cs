using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests
{
    public class DogServiceTests
    {
        private readonly Mock<IDogRepository> _dogRepositoryMock;
        private readonly DogService _dogService;

        public DogServiceTests()
        {
            _dogRepositoryMock = new Mock<IDogRepository>();
            _dogService = new DogService(_dogRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateDogAsync_WhenDogDoesNotExist_ShouldAddDog()
        {
            // Arrange
            var dogDto = new DogCreateDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 15 };

            _dogRepositoryMock.Setup(repo => repo.DogExistsAsync(dogDto.Name)).ReturnsAsync(false);

            // Act
            var result = await _dogService.CreateDogAsync(dogDto);

            // Assert
            Assert.True(result.IsSuccessful);
            _dogRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Dog>()), Times.Once);
            _dogRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDogAsync_WhenDogAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var dogDto = new DogCreateDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 15 };

            _dogRepositoryMock.Setup(repo => repo.DogExistsAsync(dogDto.Name)).ReturnsAsync(true);

            // Act
            var result = await _dogService.CreateDogAsync(dogDto);

            // Assert
            Assert.False(result.IsSuccessful);
            _dogRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Dog>()), Times.Never);
            _dogRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetDogsAsync_WhenInvalidAttribute_ShouldThrowArgumentException()
        {
            var parameters = new DogQueryParameters
            {
                Attribute = "InvalidAttribute",
                Order = "asc",
                PageNumber = 1,
                PageSize = 10
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _dogService.GetDogsAsync(parameters));
        }
    }
}
