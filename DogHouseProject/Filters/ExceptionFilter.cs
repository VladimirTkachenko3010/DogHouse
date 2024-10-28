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

            // If it is an argument error (eg invalid parameters or data)
            if (context.Exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = context.Exception.Message;
            }

            context.Result = new JsonResult(new { error = message })
            {
                StatusCode = statusCode
            };
        }
    }
}
