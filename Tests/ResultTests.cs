using Application.Utilities;
using Xunit;

namespace Tests
{
    public class ResultTests
    {
        [Fact]
        public void Failure_ShouldCreateFailedResultWithErrors()
        {
            // Act
            var result = Result.Failure("Error occurred");

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Single(result.Errors);
            Assert.Equal("Error occurred", result.Errors[0]);
            Assert.Null(result.Id);
        }

        [Fact]
        public void Success_ShouldCreateSuccessfulResultWithId()
        {
            // Act
            var result = Result.Success(1);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Empty(result.Errors);
            Assert.Equal(1, result.Id);
        }
    }
}
