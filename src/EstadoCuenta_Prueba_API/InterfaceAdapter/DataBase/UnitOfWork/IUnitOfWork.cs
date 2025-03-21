using Microsoft.Data.SqlClient;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedure, params SqlParameter[] parameters);
    Task<IEnumerable<T>> ExecuteStoredProcedureAsyncList<T>(string storedProcedure, params SqlParameter[] parameters);
}

