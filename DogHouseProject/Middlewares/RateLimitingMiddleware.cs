namespace Api.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _maxRequests;
        private int _requestCount;
        private readonly TimeSpan _timeWindow;
        private DateTime _windowStart;

        public RateLimitingMiddleware(RequestDelegate next, int maxRequestsPerSecond, TimeSpan? timeWindow = null)
        {
            _next = next;
            _maxRequests = maxRequestsPerSecond;
            _timeWindow = timeWindow ?? TimeSpan.FromSeconds(1);
            _windowStart = DateTime.UtcNow;
            _requestCount = 0;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var now = DateTime.UtcNow;

            if (now - _windowStart > _timeWindow)
            {
                // We are starting a new time interval
                _windowStart = now;
                _requestCount = 0;
            }

            if (_requestCount >= _maxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too Many Requests");
                return;
            }

            _requestCount++; // Increasing the request counter

            try
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                await _next(context);
            }
            finally
            {
                // we just decrease the counter
            }
        }
    }
}