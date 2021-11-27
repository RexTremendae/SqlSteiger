namespace SqlDataExtractor
{
    public interface IDbCommand : IAsyncDisposable
    {
        Task<IDbDataReader> ExecuteReaderAsync();
    }
}