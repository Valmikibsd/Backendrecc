using System.Diagnostics;
using CrudOperations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Shared.Logging.ErrorLogger;

public sealed class ApplicationErrorLogService(
    ICrudOperationService crudOperationService,
    ILogger<ApplicationErrorLogService> logger) : IApplicationErrorLogService
{
    private const string InsertStoredProcedure = "InsertErrorLog";

    public async Task LogAsync(ApplicationErrorLogEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        try
        {
            await crudOperationService
                .InsertUpdateDelete<Response>(InsertStoredProcedure, entry, ConnectionNames.Master)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist application error log.");
        }
    }

    internal static ApplicationErrorLogEntry CreateFromException(
        Exception exception,
        string? controllerName,
        string? actionName,
        string? requestPath,
        string? httpMethod,
        long? createdBy)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var message = exception.Message;
        var source = exception.Source;

        if (exception is SqlException sqlException &&
            !string.IsNullOrWhiteSpace(sqlException.Procedure))
        {
            message = $"{message} (Procedure: {sqlException.Procedure})";
            source = sqlException.Procedure;
        }

        var lineNumber = new StackTrace(exception, true).GetFrame(0)?.GetFileLineNumber();

        return new ApplicationErrorLogEntry(
            ControllerName: controllerName,
            ActionName: actionName,
            RequestPath: requestPath,
            HttpMethod: httpMethod,
            ExceptionType: exception.GetType().FullName,
            Message: message,
            StackTrace: exception.StackTrace,
            Source: source,
            InnerException: exception.InnerException?.ToString(),
            LineNumber: lineNumber is > 0 ? lineNumber : null,
            CreatedBy: createdBy,
            RequestBody: null);
    }
}
