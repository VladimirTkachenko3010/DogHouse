using Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Tests
{
    public class RateLimitingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenRequestLimitNotExceeded_AllowsRequest()
        {
            // Arrange
            var maxRequestsPerSecond = 1;
            var middleware = new RateLimitingMiddleware(context => Task.CompletedTask, maxRequestsPerSecond);
            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WhenRequestLimitExceeded_ReturnsTooManyRequests()
        {
            // Arrange
            var maxRequestsPerSecond = 2;
            var middleware = new RateLimitingMiddleware(context => Task.CompletedTask, maxRequestsPerSecond);

            var context1 = new DefaultHttpContext();
            var context2 = new DefaultHttpContext();
            var context3 = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context1); // First request - must pass
            await middleware.InvokeAsync(context2); // Second request - must pass
            await middleware.InvokeAsync(context3); // Third request must be blocked

            // Assert
            Assert.Equal(StatusCodes.Status200OK, context1.Response.StatusCode);
            Assert.Equal(StatusCodes.Status200OK, context2.Response.StatusCode);
            Assert.Equal(StatusCodes.Status429TooManyRequests, context3.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithResetTimeWindow_AllowsRequestAfterTimeWindow()
        {
            // Arrange
            var maxRequestsPerSecond = 1;
            var middleware = new RateLimitingMiddleware(context => Task.CompletedTask, maxRequestsPerSecond, TimeSpan.FromMilliseconds(100));

            var context1 = new DefaultHttpContext();
            var context2 = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context1); // First request - must go through

            // Wait for the end of the time interval
            await Task.Delay(150);

            await middleware.InvokeAsync(context2); // Second request - must be made after the interval ends

            // Assert
            Assert.Equal(StatusCodes.Status200OK, context1.Response.StatusCode);
            Assert.Equal(StatusCodes.Status200OK, context2.Response.StatusCode);
        }
    }
}
