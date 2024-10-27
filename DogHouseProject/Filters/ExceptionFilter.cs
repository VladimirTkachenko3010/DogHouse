using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(new { error = context.Exception.Message })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
