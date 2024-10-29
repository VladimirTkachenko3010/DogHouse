using Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using Xunit;

namespace Tests
{
    public class JsonSyntaxErrorMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenJsonIsInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            var invalidJson = "{ \"name\": \"Buddy\", \"color\": }"; // Invalid JSON
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));
            context.Response.Body = new MemoryStream();  // Installing Response.Body

            var middleware = new JsonSyntaxErrorMiddleware((innerHttpContext) => Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

            // Reading data from Response.Body
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = new StreamReader(context.Response.Body).ReadToEnd();
            Assert.Contains("Invalid JSON format. Please check your JSON syntax.", response);
        }

        [Fact]
        public async Task InvokeAsync_WhenJsonIsValid_ShouldPassToNextMiddleware()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            var validJson = "{ \"name\": \"Buddy\", \"color\": \"Brown\" }"; // Valid JSON
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(validJson));
            context.Response.Body = new MemoryStream();  // Installing Response.Body

            var nextMiddlewareMock = new Mock<RequestDelegate>();
            var middleware = new JsonSyntaxErrorMiddleware(nextMiddlewareMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            nextMiddlewareMock.Verify(m => m.Invoke(context), Times.Once);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }
    }
}
