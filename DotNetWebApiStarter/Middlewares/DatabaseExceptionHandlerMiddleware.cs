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
                _logger.LogError(ex, "Si è verificato un errore non gestito al livello del database.");
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
                    var conflictErrorResponse = new { Message = $"Errore di integrità del database: chiave duplicata. Dettagli: {exception.Message}" };
                    var conflictJsonResponse = JsonSerializer.Serialize(conflictErrorResponse);
                    _logger.LogWarning($"Richiesta fallita a causa di violazione di chiave unica: {exception.Message}");
                    await context.Response.WriteAsync(conflictJsonResponse);
                    break;
                // Aggiungi altri codici di errore SQL specifici che vuoi gestire
                // case 547: // Violation of FOREIGN KEY constraint
                //     context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //     var foreignKeyErrorResponse = new { Message = $"Errore di integrità del database: violazione di chiave esterna. Dettagli: {exception.Message}" };
                //     var foreignKeyJsonResponse = JsonSerializer.Serialize(foreignKeyErrorResponse);
                //     _logger.LogWarning($"Richiesta fallita a causa di violazione di chiave esterna: {exception.Message}");
                //     await context.Response.WriteAsync(foreignKeyJsonResponse);
                //     break;
                default:
                    // Gestisci altri errori SQL come Internal Server Error
                    _logger.LogError(exception, $"Errore SQL non gestito: Numero {exception.Number}, Messaggio: {exception.Message}");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var defaultErrorResponse = new { Message = "Si è verificato un errore del database." };
                    var defaultJsonResponse = JsonSerializer.Serialize(defaultErrorResponse);
                    await context.Response.WriteAsync(defaultJsonResponse);
                    break;
            }
        }
    }
}
