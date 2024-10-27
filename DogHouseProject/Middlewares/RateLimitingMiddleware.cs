namespace Api.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxRequests;

        public RateLimitingMiddleware(RequestDelegate next, int maxRequestsPerSecond)
        {
            _next = next;
            _maxRequests = maxRequestsPerSecond;
            _semaphore = new SemaphoreSlim(_maxRequests, _maxRequests);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_semaphore.Wait(0))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too Many Requests");
                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
