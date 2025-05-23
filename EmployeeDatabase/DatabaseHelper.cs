using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EmployeeDatabase
{
    public static class DatabaseHelper
    {
        private const string ConnectionString =
            "Server=.;Database=Test;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;";

        public static async Task<DataTable> GetQueryResultAsync(string query, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        if (parameters?.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Failed to execute query.", ex);
            }
        }

        public static async Task ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        if (parameters?.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Failed to execute non-query.", ex);
            }
        }
    }
}
