using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EmployeeDatabase
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private const string HierarchyQuery = @"
            WITH EmployeeHierarchy AS (
                SELECT * FROM Employee WHERE Id = @Id
                UNION ALL
                SELECT e.* FROM Employee e
                JOIN EmployeeHierarchy eh ON e.ManagerId = eh.Id
            )
            SELECT * FROM EmployeeHierarchy";

        private const string UpdateEnableQuery = "UPDATE Employee SET Enable = @Enable WHERE Id = @Id";

        public async Task<DataTable> GetEmployeeHierarchyAsync(int id)
        {
            return await DatabaseHelper.GetQueryResultAsync(HierarchyQuery, new SqlParameter("@Id", id));
        }

        public async Task UpdateEmployeeEnableAsync(int id, bool enable)
        {
            var parameters = new[]
            {
                new SqlParameter("@Enable", enable),
                new SqlParameter("@Id", id)
            };
            await DatabaseHelper.ExecuteNonQueryAsync(UpdateEnableQuery, parameters);
        }
    }
}
