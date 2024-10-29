using Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Xunit;

namespace Tests
{
    public class ExceptionFilterTests
    {
        [Fact]
        public void OnException_WithArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var exceptionFilter = new ExceptionFilter();
            var exceptionContext = new ExceptionContext(
                new ActionContext(Mock.Of<HttpContext>(), Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(), Mock.Of<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor>()),
                new List<IFilterMetadata>());
            exceptionContext.Exception = new ArgumentException("Invalid argument");

            // Act
            exceptionFilter.OnException(exceptionContext);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(exceptionContext.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, jsonResult.StatusCode);

            var resultValue = Assert.IsType<Dictionary<string, string>>(jsonResult.Value);
            Assert.Equal("Invalid argument", resultValue["error"]);
        }

        [Fact]
        public void OnException_WithGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var exceptionFilter = new ExceptionFilter();
            var exceptionContext = new ExceptionContext(
                new ActionContext(Mock.Of<HttpContext>(), Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(), Mock.Of<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor>()),
                new List<IFilterMetadata>());
            exceptionContext.Exception = new Exception("Something went wrong");

            // Act
            exceptionFilter.OnException(exceptionContext);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(exceptionContext.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, jsonResult.StatusCode);

            var resultValue = Assert.IsType<Dictionary<string, string>>(jsonResult.Value);
            Assert.Equal("An unexpected error occurred.", resultValue["error"]);
        }
    }
}
