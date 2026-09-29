namespace Users.WebAPI.Common.Middlewares
{
    /// <summary>
    /// Middleware that monitors the performance of HTTP requests by measuring their execution time.
    /// Logs a warning if the processing time exceeds a predefined threshold.
    /// </summary>
    public class PerformanceMiddleware(RequestDelegate requestDelegate, ILogger<PerformanceMiddleware> logger)
    {
        private const int ThresholdInMilliseconds = 500;

        // Methods:
        /// <summary>
        /// Processes an incoming HTTP request and logs a warning if the request processing time exceeds a predefined threshold.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> representing the current HTTP request.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            DateTime start = DateTime.UtcNow;

            await requestDelegate(httpContext);

            TimeSpan duration = DateTime.UtcNow - start;

            if(duration.TotalMicroseconds > ThresholdInMilliseconds)
            {
                logger.LogWarning(
                    "##### [Users.WebAPI.Common.Middlewares.PerformanceMiddleware.cs] [InvokeAsync()] Slow request detected: {method} {path} took {duration}ms #####",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    duration.TotalMilliseconds
                );
            }
        }
    }
}