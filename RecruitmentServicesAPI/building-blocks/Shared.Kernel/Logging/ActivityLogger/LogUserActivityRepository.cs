using CrudOperations;

namespace Shared.Logging.ActivityLogger;

public class LogUserActivityRepository(ICrudOperationService crudOperationService) : ILogUserActivityRepository
{
    private const string SaveStoredProcedure = "UspSaveUserActivityLog";
    private readonly ICrudOperationService _crudOperationService = crudOperationService;

    public async Task SaveLogAsync(LogUserActivity logUserActivity)
    {
        ArgumentNullException.ThrowIfNull(logUserActivity);

        await _crudOperationService
            .InsertUpdateDelete<Response>(SaveStoredProcedure, logUserActivity, ConnectionNames.Master)
            .ConfigureAwait(false);
    }
}
