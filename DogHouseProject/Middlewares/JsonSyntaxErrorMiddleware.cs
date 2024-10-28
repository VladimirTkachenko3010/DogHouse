using System.Text.Json;

namespace Api.Middlewares
{
    public class JsonSyntaxErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public JsonSyntaxErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
            {
                context.Request.EnableBuffering();

                try
                {
                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    JsonDocument.Parse(body); // Checking JSON syntax

                    // We return the stream to the beginning so that the request can be read again
                    context.Request.Body.Position = 0;
                }
                catch (JsonException ex) when (ex.Path == null) // We catch only JSON syntax errors
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new
                    {
                        Errors = new[]
                        {
                        new { Message = "Invalid JSON format. Please check your JSON syntax." }
                    }
                    };

                    var jsonResponse = JsonSerializer.Serialize(errorResponse);
                    await context.Response.WriteAsync(jsonResponse);
                    return; // Terminate processing if JSON is syntactically invalid
                }
            }

            await _next(context); // We pass the request on if the syntax is correct
        }
    }

}
