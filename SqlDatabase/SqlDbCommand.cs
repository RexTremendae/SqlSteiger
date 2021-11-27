using Microsoft.Data.SqlClient;

namespace SqlDataExtractor.SqlDatabase
{
    public class SqlDbCommand : IDbCommand
    {
        private readonly SqlCommand _sqlCommand;

        public SqlDbCommand(SqlCommand sqlCommand)
        {
            _sqlCommand = sqlCommand;
        }

        public async ValueTask DisposeAsync()
        {
            await _sqlCommand.DisposeAsync();
        }

        public async Task<IDbDataReader> ExecuteReaderAsync()
        {
            return new SqlDbReader(await _sqlCommand.ExecuteReaderAsync());
        }
    }
}