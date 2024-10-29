using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var statusCode = StatusCodes.Status500InternalServerError; // Default 500
            var message = "An unexpected error occurred."; // General error

            if (context.Exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = context.Exception.Message;
            }

            // Return a strongly typed dictionary instead of an anonymous object
            context.Result = new JsonResult(new Dictionary<string, string> { { "error", message } })
            {
                StatusCode = statusCode
            };
        }
    }
}
