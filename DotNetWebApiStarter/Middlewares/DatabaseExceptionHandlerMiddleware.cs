using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace DotNetWebApiStarter.Middlewares
{
    public class DatabaseExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseExceptionHandlerMiddleware> _logger;

        public DatabaseExceptionHandlerMiddleware(RequestDelegate next, ILogger<DatabaseExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (SqlException ex)
            {
                await HandleSqlExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred at the database level.");
                throw;
            }
        }

        private async Task HandleSqlExceptionAsync(HttpContext context, SqlException exception)
        {
            context.Response.ContentType = "application/json";

            switch (exception.Number)
            {
                case 2601: // Violation of UNIQUE KEY constraint
                case 2627: // Violation of PRIMARY KEY constraint (which is also unique)
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    var conflictErrorResponse = new { Message = $"Database integrity error: duplicate key. Details: {exception.Message}" };
                    var conflictJsonResponse = JsonSerializer.Serialize(conflictErrorResponse);
                    _logger.LogWarning($"Request failed due to unique key violation: {exception.Message}");
                    await context.Response.WriteAsync(conflictJsonResponse);
                    break;
                 case 547: // Violation of FOREIGN KEY constraint
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var foreignKeyErrorResponse = new { Message = $"Database integrity error: foreign key violation. Details: {exception.Message}" };
                    var foreignKeyJsonResponse = JsonSerializer.Serialize(foreignKeyErrorResponse);
                    _logger.LogWarning($"Request failed due to foreign key violation: {exception.Message}");
                    await context.Response.WriteAsync(foreignKeyJsonResponse);
                    break;
                default:
                    _logger.LogError(exception, $"Unhandled SQL Error: Number {exception.Number}, Message: {exception.Message}");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var defaultErrorResponse = new { Message = "A database error occurred." };
                    var defaultJsonResponse = JsonSerializer.Serialize(defaultErrorResponse);
                    await context.Response.WriteAsync(defaultJsonResponse);
                    break;
            }
        }
    }
}
