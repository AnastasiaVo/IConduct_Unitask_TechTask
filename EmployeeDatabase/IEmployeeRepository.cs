using System.Data;
using System.Threading.Tasks;

namespace EmployeeDatabase
{
    public interface IEmployeeRepository
    {
        Task<DataTable> GetEmployeeHierarchyAsync(int id);

        Task UpdateEmployeeEnableAsync(int id, bool enable);
    }
}
