using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
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
                // Step 1: Ask for employee to output
                Console.WriteLine("Enter employee ID or name to fetch (e.g., '3' or 'Nir'): ");
                string input = Console.ReadLine().Trim();
                int employeeId = -1;
                bool byId = int.TryParse(input, out employeeId);

                // Step 2: Fetch all employees to find the target
                string allEmployeesJson = await GetAllEmployeesAsync();
                JArray allEmployees = JArray.Parse(allEmployeesJson);
                JObject targetEmployee = null;

                if (byId)
                {
                    targetEmployee = await GetEmployeeByIdAsync(employeeId);
                }
                else
                {
                    var employee = allEmployees.FirstOrDefault(e => e["Name"]?.ToString().Equals(input, StringComparison.OrdinalIgnoreCase) == true);
                    if (employee != null)
                    {
                        employeeId = employee["ID"].Value<int>();
                        targetEmployee = await GetEmployeeByIdAsync(employeeId);
                    }
                }

                if (targetEmployee == null)
                {
                    Console.WriteLine("Employee not found.");
                    return;
                }

                // Step 3: Display the JSON
                string formattedJson = targetEmployee.ToString();
                Console.WriteLine("Employee Data:");
                Console.WriteLine(formattedJson);

                // Save to file
                File.WriteAllText("employee.json", formattedJson);
                Console.WriteLine("Employee data saved to employee.json");

                // Step 4: Ask if user wants to disable an employee
                Console.WriteLine("\nDo you want to disable an employee? (yes/no): ");
                string disableChoice = Console.ReadLine().Trim().ToLower();
                if (disableChoice == "yes")
                {
                    Console.WriteLine("Enter the employee ID to disable: ");
                    if (int.TryParse(Console.ReadLine().Trim(), out int disableId))
                    {
                        Console.WriteLine("\nDisabling employee with ID " + disableId + "...");
                        await EnableEmployeeAsync(disableId, 0);
                        Console.WriteLine("Employee with ID " + disableId + " has been disabled.");

                        // Fetch and display updated data
                        targetEmployee = await GetEmployeeByIdAsync(employeeId);
                        if (targetEmployee != null)
                        {
                            formattedJson = targetEmployee.ToString();
                            Console.WriteLine("\nEmployee Data After Update:");
                            Console.WriteLine(formattedJson);

                            File.WriteAllText("employee_updated.json", formattedJson);
                            Console.WriteLine("Updated employee data saved to employee_updated.json");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID entered.");
                    }
                }
                else if (disableChoice != "no")
                {
                    Console.WriteLine("Invalid choice. Please enter 'yes' or 'no'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task<JObject> GetEmployeeByIdAsync(int id)
        {
            string url = $"http://localhost:64014/EmployeeService.svc/GetEmployeeById?id={id}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JObject.Parse(json);
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

        static async Task<string> GetAllEmployeesAsync()
        {
            const string url = "http://localhost:64014/EmployeeService.svc/GetAllEmployees";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
