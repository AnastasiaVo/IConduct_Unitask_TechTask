using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeServiceClient
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string serviceUrlBase = ConfigurationManager.AppSettings["ServiceUrlBase"]
                    ?? throw new ConfigurationErrorsException("Service URL 'ServiceUrlBase' not found.");
                var client = new EmployeeServiceClient(serviceUrlBase);

                Console.WriteLine("Enter employee ID to fetch (e.g., '3'): ");
                if (!int.TryParse(Console.ReadLine()?.Trim(), out int employeeId))
                {
                    Console.WriteLine("Invalid ID entered.");
                    return;
                }

                string employeeData = await client.GetEmployeeByIdAsync(employeeId);
                if (string.IsNullOrEmpty(employeeData))
                {
                    Console.WriteLine("No data found.");
                    return;
                }

                string formattedJson = JToken.Parse(employeeData).ToString(Formatting.Indented);
                Console.WriteLine(formattedJson);
                File.WriteAllText("employee.json", formattedJson);
                Console.WriteLine("Employee data saved to employee.json");

                Console.WriteLine("\nDo you want to disable this employee? (yes/no): ");
                string disableChoice = Console.ReadLine()?.Trim().ToLower();
                if (disableChoice == "yes")
                {
                    Console.WriteLine("Enter employee ID to disable (e.g., '3'): ");
                    if (!int.TryParse(Console.ReadLine()?.Trim(), out int employeeIdToDisable))
                    {
                        Console.WriteLine("Invalid ID entered.");
                        return;
                    }

                    await client.EnableEmployeeAsync(employeeIdToDisable, false);
                    Console.WriteLine($"Employee with ID {employeeIdToDisable} has been disabled.");

                    employeeData = await client.GetEmployeeByIdAsync(employeeIdToDisable);
                    if (!string.IsNullOrEmpty(employeeData))
                    {
                        formattedJson = JToken.Parse(employeeData).ToString(Formatting.Indented);
                        Console.WriteLine(formattedJson);
                        File.WriteAllText("employee.json", formattedJson);
                        Console.WriteLine("Updated employee data saved to employee.json");
                    }
                }
                else if (disableChoice != "no")
                {
                    Console.WriteLine("Invalid choice. Please enter 'yes' or 'no'.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
