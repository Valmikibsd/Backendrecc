using System.Data;

namespace CrudOperations;

public interface ICrudOperationService
{
    Task<T?> InsertUpdateDelete<T>(string storedProcedureName, object spParamsPocoMapper, string? connectionName = null);

    Task<Response<T>> GetSingleRecord<T>(string storedProcedureName, object spParamsPocoMapper, string? connectionName = null);

    Task<ResponseList<T>> GetList<T>(string storedProcedureName, object spParamsPocoMapper, string? connectionName = null);

    Task<DynamicResponse> BindDynamicLists(string storedProcedureName, object parameters, List<string> resultSetNames, string? connectionName = null);

    Task<Response> BulkInsert(DataTable dataTable, string destinationTableName, string? connectionName = null);

    Task<T?> InsertWithTransaction<T>(
       string storedProcedureName,
       object spParamsPocoMapper,
       IDbConnection connection,
       IDbTransaction transaction);


    IDbTransaction BeginTransaction(
        IDbConnection connection);

    IDbConnection CreateConnection(string? connectionName);
}
