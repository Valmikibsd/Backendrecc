namespace Shared.Logging.ActivityLogger;

public interface ILogUserActivityRepository
{
    Task SaveLogAsync(LogUserActivity logUserActivity);
}
