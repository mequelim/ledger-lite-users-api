using Users.WebAPI.Common.Middlewares;

namespace Users.WebAPI.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring middleware in an ASP.NET Core application.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Configures the application to use a predefined set of middlewares for handling HTTP requests.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
        /// <returns>The configured <see cref="IApplicationBuilder"/> instance.</returns>
        /// <remarks>
        /// This method adds the following middlewares to the pipeline in the specified order:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="CorrelationIdMiddleware"/>: ensures every HTTP request has a unique correlation ID for tracking and logging purposes.</description>
        /// </item>
        /// <item>
        /// <description><see cref="RequestLoggingMiddleware"/>: logs details about incoming HTTP requests, including method, path, and processing duration.</description>
        /// </item>
        /// <item>
        /// <description><see cref="PerformanceMiddleware"/>: monitors the performance of HTTP requests by measuring execution time and logging warnings for slow requests.</description>
        /// </item>
        /// <item>
        /// <description><see cref="ExceptionHandlingMiddleware"/>: handles unhandled exceptions by logging error details and returning a standardized JSON response.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
            applicationBuilder.UseMiddleware<RequestLoggingMiddleware>();
            applicationBuilder.UseMiddleware<PerformanceMiddleware>();
            applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();

            return applicationBuilder;
        }
    }
}