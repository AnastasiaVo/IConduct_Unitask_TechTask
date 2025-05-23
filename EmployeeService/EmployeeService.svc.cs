using InterviewConsole;
using System;
using System.Collections.Generic;
using System.Data;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmployeeService.svc or EmployeeService.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        private const string connectionString
            = "Server=.;Database=Test;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;";

        public EmployeeEntity GetEmployeeById(int id)
        {
            DataTable dt = DatabaseHelper.GetQueryResult("SELECT * FROM Employee");

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

            return rootEmployee;
        }

        public void EnableEmployee(int id, int enable)
        {
            if (enable != 0 && enable != 1)
            {
                throw new ArgumentException("Enable parameter must be 0 or 1.");
            }

            string query = "UPDATE Employee SET Enable = @Enable WHERE ID = @ID";
            var parameters = new[]
            {
                new System.Data.SqlClient.SqlParameter("@Enable", enable),
                new System.Data.SqlClient.SqlParameter("@ID", id)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }
    }
}