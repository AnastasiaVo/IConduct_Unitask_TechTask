using EmployeeDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmployeeService.svc or EmployeeService.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService()
        {
            _repository = new EmployeeRepository();
        }
        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<EmployeeResponse> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be positive", nameof(id));
            }

            try
            {
                var dt = await _repository.GetEmployeeHierarchyAsync(id);
                if (dt.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"Employee with Id {id} not found.");
                }

                var employeeDict = BuildEmployeeDictionary(dt);
                if (!employeeDict.TryGetValue(id, out EmployeeEntity targetEmployee))
                {
                    throw new KeyNotFoundException($"Employee with Id {id} not found.");
                }

                BuildHierarchy(employeeDict);
                return new EmployeeResponse { Employee = new List<EmployeeEntity> { targetEmployee } };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve employee data.", ex);
            }
        }

        public async Task EnableEmployeeAsync(int id, bool enable)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be positive", nameof(id));
            }

            try
            {
                await _repository.UpdateEmployeeEnableAsync(id, enable);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update employee status.", ex);
            }
        }

        private static Dictionary<int, EmployeeEntity> BuildEmployeeDictionary(DataTable dt)
        {
            var employeeDict = new Dictionary<int, EmployeeEntity>();
            foreach (DataRow row in dt.Rows)
            {
                var emp = EmployeeMapper.Map(row);
                employeeDict[emp.Id] = emp;
            }
            return employeeDict;
        }

        private static void BuildHierarchy(Dictionary<int, EmployeeEntity> employeeDict)
        {
            foreach (var employee in employeeDict.Values)
            {
                if (employee.ManagerId.HasValue && employeeDict.ContainsKey(employee.ManagerId.Value))
                {
                    employeeDict[employee.ManagerId.Value].Employees.Add(employee);
                }
            }
        }
    }
}