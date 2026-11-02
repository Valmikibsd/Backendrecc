using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;

namespace CrudOperations;

public class CrudOperationDataAccess : ICrudOperationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CrudOperationDataAccess>? _logger;

    public CrudOperationDataAccess(IConfiguration configuration, ILogger<CrudOperationDataAccess>? logger = null)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger;
    }

    public Task<T?> InsertUpdateDelete<T>(string storedProcedureName, object spParamsPocoMapper, string? connectionName = null)
    {
        try
        {
            return QueryFirstOrDefaultAsync<T>(storedProcedureName, MapParameters(spParamsPocoMapper), connectionName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing InsertUpdateDelete for stored procedure {StoredProcedureName}", storedProcedureName);
            throw;
        }
    }

    public async Task<Response<T>> GetSingleRecord<T>(string storedProcedureName, object spParamsPocoMapper, string? connectionName = null)
    {
        try
        {
            using var connection = CreateConnection(connectionName);
            using var result = await connection.QueryMultipleAsync(
                storedProcedureName,
                MapParameters(spParamsPocoMapper),
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            var data = await result.ReadFirstOrDefaultAsync<T>().ConfigureAwait(false);
            var response = await result.ReadFirstOrDefaultAsync<Response<T>>().ConfigureAwait(false);

            if (data != null && response != null)
            {
                response.Data = data;
                return response;
            }

            return new Response<T>
            {
                Status = false,
                Message = "No data returned."
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing GetSingleRecord for stored procedure {StoredProcedureName}", storedProcedureName);
            throw;
        }
    }

    public async Task<ResponseList<T>> GetList<T>(string storedProcedureName, object spParamsPocoMapper, string? connectionName = null)
    {
        try
        {
            using var connection = CreateConnection(connectionName);
            using var result = await connection.QueryMultipleAsync(
                storedProcedureName,
                MapParameters(spParamsPocoMapper),
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            var data = (await result.ReadAsync<T>().ConfigureAwait(false)).ToList();
            var response = await result.ReadFirstOrDefaultAsync<ResponseList<T>>().ConfigureAwait(false);

            if (response != null)
            {
                response.Data = data;
                if (response.TotalRecords <= 0)
                {
                    response.TotalRecords = data.Count;
                }
                return response;
            }

            return new ResponseList<T>
            {
                Status = false,
                Message = "No data returned.",
                Data = data,
                TotalRecords = data.Count
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing GetList for stored procedure {StoredProcedureName}", storedProcedureName);
            throw;
        }
    }

    public async Task<DynamicResponse> BindDynamicLists(string storedProcedureName, object parameters, List<string> resultSetNames, string? connectionName = null)
    {
        try
        {
            using var connection = CreateConnection(connectionName);
            using var result = await connection.QueryMultipleAsync(
                storedProcedureName,
                MapParameters(parameters),
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            var response = new DynamicResponse();
            var index = 0;

            while (!result.IsConsumed)
            {
                var dataSet = (await result.ReadAsync().ConfigureAwait(false)).ToList();
                var name = resultSetNames.ElementAtOrDefault(index) ?? $"DataSet{index + 1}";
                response.NamedDataSets[name] = dataSet;
                index++;
            }

            if (response.NamedDataSets.LastOrDefault().Value is List<dynamic> statusResult && statusResult.Count > 0)
            {
                dynamic statusData = statusResult[0];
                response.Status = IsSuccessStatus(statusData?.Status);
                response.Message = statusData?.Message ?? "No message returned";
                response.NamedDataSets.Remove(response.NamedDataSets.Last().Key);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing BindDynamicLists for stored procedure {StoredProcedureName}", storedProcedureName);
            throw;
        }
    }

    public async Task<Response> BulkInsert(DataTable dataTable, string destinationTableName, string? connectionName = null)
    {
        try
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return new Response
                {
                    Status = false,
                    Message = "No rows to import."
                };
            }

            if (string.IsNullOrWhiteSpace(destinationTableName))
            {
                throw new ArgumentException("Destination table name is required.", nameof(destinationTableName));
            }

            await using var connection = (SqlConnection)CreateConnection(connectionName);
            await connection.OpenAsync().ConfigureAwait(false);

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = destinationTableName,
                BatchSize = 5000,
                BulkCopyTimeout = 0
            };

            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable).ConfigureAwait(false);

            return new Response
            {
                Status = true,
                Message = $"{dataTable.Rows.Count} rows imported successfully."
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing BulkInsert for destination table {DestinationTableName}", destinationTableName);
            throw;
        }
    }

    private async Task<T?> QueryFirstOrDefaultAsync<T>(string storedProcedureName, DynamicParameters parameters, string? connectionName)
    {
        try
        {
            using var connection = CreateConnection(connectionName);
            return await connection.QueryFirstOrDefaultAsync<T>(
                storedProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing QueryFirstOrDefaultAsync for stored procedure {StoredProcedureName}", storedProcedureName);
            throw;
        }
    }

    public IDbTransaction BeginTransaction(IDbConnection connection)
    {
        try
        {
            return connection.BeginTransaction();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error beginning transaction");
            throw;
        }
    }

    public async Task<T?> InsertWithTransaction<T>(
        string storedProcedureName,
        object spParamsPocoMapper,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        try
        {
            return await connection.QueryFirstOrDefaultAsync<T>(
                storedProcedureName,
                MapParameters(spParamsPocoMapper),
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            ).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing InsertWithTransaction for stored procedure {StoredProcedureName}", storedProcedureName);
            throw;
        }
    }

    public IDbConnection CreateConnection(string? connectionName)
    {
        try
        {
            return new SqlConnection(ResolveConnectionString(connectionName));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating connection for {ConnectionName}", connectionName);
            throw;
        }
    }

    private string ResolveConnectionString(string? connectionName)
    {
        try
        {
            var name = string.IsNullOrWhiteSpace(connectionName) ? ConnectionNames.Default : connectionName;
            var connectionString = _configuration.GetConnectionString(name);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{name}' is not configured.");
            }

            return connectionString;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error resolving connection string for {ConnectionName}", connectionName);
            throw;
        }
    }

    private DynamicParameters MapParameters(object model)
    {
        try
        {
            if (model is DynamicParameters dynamicParameters)
            {
                return dynamicParameters;
            }

            var parameters = new DynamicParameters();
            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.CustomAttributes.Any(x => x.AttributeType.Name.Contains("Ignore", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var value = property.GetValue(model);
                if (property.PropertyType == typeof(DataTable))
                {
                    parameters.Add($"@{property.Name}", ((DataTable)value!).AsTableValuedParameter());
                }
                else
                {
                    parameters.Add($"@{property.Name}", value);
                }
            }

            return parameters;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error mapping parameters for model {ModelType}", model?.GetType().Name);
            throw;
        }
    }

    private static bool IsSuccessStatus(object? status) =>
        status switch
        {
            bool boolStatus => boolStatus,
            int intStatus => intStatus == 1,
            long longStatus => longStatus == 1,
            string stringStatus => stringStatus == "1" || string.Equals(stringStatus, "true", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
}
