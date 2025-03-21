using Infrastructure.DataBase.DBContext;
using Microsoft.Data.SqlClient;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedure, params SqlParameter[] parameters)
    {
        return await _context.ExecuteStoredProcedureAsync<T>(storedProcedure, parameters);
    }

    public async Task<IEnumerable<T>> ExecuteStoredProcedureAsyncList<T>(string storedProcedure, params SqlParameter[] parameters)
    {
        return await _context.ExecuteStoredProcedureAsyncList<T>(storedProcedure, parameters);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

