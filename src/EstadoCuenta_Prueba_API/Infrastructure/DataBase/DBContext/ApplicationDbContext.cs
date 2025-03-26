using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace Infrastructure.DataBase.DBContext
{
    public class ApplicationDbContext : DbContext
    {

        private readonly SqlConnection _connection;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, SqlConnection connection) : base(options)
        {
            _connection = connection;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedure, params SqlParameter[] parameters)
        {
            T result = default;

            try
            {
                using (var command = new SqlCommand(storedProcedure, _connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    _connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result = MapReaderToEntity<T>(reader);
                        }
                    }
                    _connection.Close();
                }
            }
            catch(Exception ex)
            {
                _connection.Close();
            }
            return result;
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsyncList<T>(string storedProcedure, params SqlParameter[] parameters)
        {
            var result = new List<T>();

            try
            {
                using (var command = new SqlCommand(storedProcedure, _connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    _connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Assuming you have a method to map the data reader to your entity
                            result.Add(MapReaderToEntity<T>(reader));
                        }
                    }
                    _connection.Close();
                }
            }
            catch(Exception ex)
            {
                _connection.Close();
            }
            return result;
        }
        private T MapReaderToEntity<T>(SqlDataReader reader)
        {
            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!reader.HasColumn(property.Name) || reader[property.Name] == DBNull.Value)
                    continue;

                property.SetValue(entity, reader[property.Name]);
            }

            return entity;
        }

    }
    public static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
