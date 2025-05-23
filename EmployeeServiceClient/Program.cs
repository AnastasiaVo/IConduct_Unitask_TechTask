using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceClient
{
    static class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Fetching employee with ID 3...");
                string employeeJson = await GetEmployeeByIdAsync(3);
                JObject jsonObject = JObject.Parse(employeeJson);
                string formattedJson = jsonObject.ToString();
                Console.WriteLine("Employee Data:");
                Console.WriteLine(formattedJson);

                File.WriteAllText("employee.json", formattedJson);
                Console.WriteLine("Employee data saved to employee.json");

                Console.WriteLine($"Root Employee Name: {jsonObject["Name"]}");

                Console.WriteLine("\nDisabling employee with ID 3...");
                await EnableEmployeeAsync(3, 0);
                Console.WriteLine("Employee with ID 3 has been disabled.");

                Console.WriteLine("\nFetching employee with ID 3 again...");
                employeeJson = await GetEmployeeByIdAsync(3);
                jsonObject = JObject.Parse(employeeJson);
                formattedJson = jsonObject.ToString();
                Console.WriteLine("Employee Data After Update:");
                Console.WriteLine(formattedJson);

                File.WriteAllText("employee_updated.json", formattedJson);
                Console.WriteLine("Updated employee data saved to employee_updated.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task<string> GetEmployeeByIdAsync(int id)
        {
            string url = $"http://localhost:64014/EmployeeService.svc/GetEmployeeById?id={id}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        static async Task EnableEmployeeAsync(int id, int enable)
        {
            string url = $"http://localhost:64014/EmployeeService.svc/EnableEmployee?id={id}";
            var content = new StringContent(
                $"{{\"enable\": {enable}}}",
                Encoding.UTF8,
                "application/json"
            );
            HttpResponseMessage response = await client.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
