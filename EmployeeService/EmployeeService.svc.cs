using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmployeeService.svc or EmployeeService.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        private const string connectionString
            = "Server=.;Database=Test;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;";

        public JObject GetEmployeeById(int id)
        {
            DataTable dt = EmployeeRepository.GetQueryResult("SELECT * FROM Employee");

            EmployeeEntity rootEmployee = null;
            var employeeDict = new Dictionary<int, EmployeeEntity>();

            foreach (DataRow row in dt.Rows)
            {
                var employee = new EmployeeEntity
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    ManagerId = row["ManagerId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ManagerId"]),
                    Enable = Convert.ToBoolean(row["Enable"])
                };
                employeeDict[employee.Id] = employee;

                if (employee.Id == id)
                {
                    rootEmployee = employee;
                }
            }

            if (rootEmployee == null)
            {
                return null;
            }

            foreach (var employee in employeeDict.Values)
            {
                if (employee.ManagerId.HasValue && employeeDict.ContainsKey(employee.ManagerId.Value))
                {
                    employeeDict[employee.ManagerId.Value].Employees.Add(employee);
                }
            }

            var allEmployees = new List<EmployeeEntity>();
            var visited = new HashSet<int>();

            void CollectEmployees(EmployeeEntity employee)
            {
                if (employee == null || visited.Contains(employee.Id)) return;
                visited.Add(employee.Id);

                foreach (var emp in employee.Employees)
                {
                    CollectEmployees(emp);
                    if (emp != rootEmployee)
                    {
                        allEmployees.Add(emp);
                    }
                }
            }

            CollectEmployees(rootEmployee);
            allEmployees.Add(rootEmployee);

            var result = new JObject
            {
                ["Employees"] = new JArray(allEmployees.Select(e => JObject.FromObject(e))),
                ["Root"] = JObject.FromObject(rootEmployee)
            };

            return result;
        }

        public void EnableEmployee(int id, int enable)
        {
            if (enable != 0 && enable != 1)
            {
                throw new ArgumentException("Enable parameter must be 0 or 1.");
            }

            const string query = "UPDATE Employee SET Enable = @Enable WHERE ID = @ID";
            var parameters = new[]
            {
                new System.Data.SqlClient.SqlParameter("@Enable", enable),
                new System.Data.SqlClient.SqlParameter("@ID", id)
            };
            EmployeeRepository.ExecuteNonQuery(query, parameters);
        }

        public List<EmployeeEntity> GetAllEmployees()
        {
            DataTable dt = EmployeeRepository.GetQueryResult("SELECT * FROM Employee");
            var allEmployees = new List<EmployeeEntity>();

            foreach (DataRow row in dt.Rows)
            {
                var employee = new EmployeeEntity
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    ManagerId = row["ManagerId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ManagerId"]),
                    Enable = Convert.ToBoolean(row["Enable"])
                };
                allEmployees.Add(employee);
            }

            return allEmployees;
        }
    }
}