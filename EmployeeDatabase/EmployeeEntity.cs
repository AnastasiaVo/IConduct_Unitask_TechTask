using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EmployeeDatabase
{
    [DataContract]
    public class EmployeeEntity
    {
        [DataMember(Name = "Id", Order = 1)]
        public int Id { get; set; }

        [DataMember(Name = "Name", Order = 2)]
        public string Name { get; set; }

        [DataMember(Name = "ManagerId", Order = 3)]
        public int? ManagerId { get; set; }

        [DataMember(Name = "Enable", Order = 4)]
        public bool Enable { get; set; }

        [DataMember(Name = "Employees", Order = 5)]
        public List<EmployeeEntity> Employees { get; set; }

        public EmployeeEntity()
        {
            Employees = new List<EmployeeEntity>();
        }
    }
}