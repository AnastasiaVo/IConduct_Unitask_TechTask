using System;
using System.Data;

namespace EmployeeDatabase
{
    public static class EmployeeMapper
    {
        public static EmployeeEntity Map(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var employee = new EmployeeEntity
            {
                Id = row.Field<int>("Id"),
                Name = row.Field<string>("Name")?.Trim() ?? string.Empty,
                Enable = row.Field<bool>("Enable")
            };

            if (row.Table.Columns.Contains("ManagerId") && row["ManagerId"] != DBNull.Value)
            {
                employee.ManagerId = row.Field<int>("ManagerId");
            }
            else
            {
                employee.ManagerId = null;
            }

            return employee;
        }
    }
}
