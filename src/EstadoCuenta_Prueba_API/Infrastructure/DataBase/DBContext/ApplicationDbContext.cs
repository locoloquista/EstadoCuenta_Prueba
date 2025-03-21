using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuraciones adicionales
        }

        public async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedure, params SqlParameter[] parameters)
        {
            return await this.ExecuteStoredProcedureAsync<T>(storedProcedure, parameters);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsyncList<T>(string storedProcedure, params SqlParameter[] parameters)
        {
            return await this.ExecuteStoredProcedureAsyncList<T>(storedProcedure, parameters);
        }
    }
}
