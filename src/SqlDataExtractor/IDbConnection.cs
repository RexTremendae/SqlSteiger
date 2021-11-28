namespace SqlDataExtractor;

public interface IDbConnection : IAsyncDisposable
{
    Task OpenAsync();
    Task<IDbDataReader> ExecuteReaderAsync(string commandText);
}
