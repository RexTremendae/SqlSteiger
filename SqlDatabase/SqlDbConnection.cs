using Microsoft.Data.SqlClient;

namespace SqlDataExtractor.SqlDatabase
{
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

        public IDbCommand CreateCommand(string commandText)
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = commandText;
            return new SqlDbCommand(command);
        }
    }
}
