namespace Shared.Logging.ErrorLogger;

public sealed record ApplicationErrorLogEntry(
    string? ControllerName,
    string? ActionName,
    string? RequestPath,
    string? HttpMethod,
    string? ExceptionType,
    string Message,
    string? StackTrace,
    string? Source,
    string? InnerException,
    int? LineNumber,
    long? CreatedBy,
    string? RequestBody);
