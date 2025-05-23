using System.Runtime.Serialization;

namespace EmployeeDatabase
{
    [DataContract]
    public class EmployeeEntity
    {
        [DataMember]
        public int Id { get; set; }

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name?.Trim();
            set => _name = value;
        }

        [DataMember]
        public int? ManagerId { get; set; }

        [DataMember]
        public bool Enable { get; set; }

        [DataMember]
        public List<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
    }
}