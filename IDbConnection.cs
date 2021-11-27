namespace SqlDataExtractor
{
    public interface IDbConnection : IAsyncDisposable
    {
        Task OpenAsync();
        IDbCommand CreateCommand(string commandText);
    }
}
