using EmployeeDatabase;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EmployeeService
{
    public class EmployeeResponse
    {
        [JsonProperty("Employee")]
        public List<EmployeeEntity> Employee { get; set; }
    }
}