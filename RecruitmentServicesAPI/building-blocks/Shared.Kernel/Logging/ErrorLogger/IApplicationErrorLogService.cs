namespace Shared.Logging.ErrorLogger;

public interface IApplicationErrorLogService
{
    Task LogAsync(ApplicationErrorLogEntry entry, CancellationToken cancellationToken = default);
}
