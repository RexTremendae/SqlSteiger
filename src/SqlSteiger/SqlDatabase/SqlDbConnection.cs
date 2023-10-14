namespace SqlSteiger.SqlDatabase;

using Microsoft.Data.SqlClient;

public class SqlDbConnection : IDbConnection
{
    private readonly SqlConnection _sqlConnection;

    public SqlDbConnection(string connectionString)
    {
        _sqlConnection = new SqlConnection(connectionString);
    }

    public SqlDbConnection(SqlConnectionStringBuilder connectionStringBuilder)
        : this(connectionStringBuilder.ToString())
    {
    }

    public async Task OpenAsync()
    {
        await _sqlConnection.OpenAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _sqlConnection.DisposeAsync();
    }

    public async Task<IDbDataReader> ExecuteReaderAsync(string commandText)
    {
        await using var command = _sqlConnection.CreateCommand();
        command.CommandText = commandText;
        return new SqlDbDataReader(await command.ExecuteReaderAsync());
    }
}
