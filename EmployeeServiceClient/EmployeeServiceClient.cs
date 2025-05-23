using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceClient
{
    public class EmployeeServiceClient
    {
        private readonly HttpClient _client;
        private readonly string _serviceUrlBase;

        public EmployeeServiceClient(string serviceUrlBase, HttpClient client = null)
        {
            _serviceUrlBase = serviceUrlBase ?? throw new ArgumentNullException(nameof(serviceUrlBase));
            _client = client ?? new HttpClient();
        }

        public async Task<string> GetEmployeeByIdAsync(int id)
        {
            string url = $"{_serviceUrlBase}/GetEmployeeById?id={id}";
            HttpResponseMessage response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Status code: {response.StatusCode}");
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task EnableEmployeeAsync(int id, bool enable)
        {
            string url = $"{_serviceUrlBase}/EnableEmployee?id={id}";
            var requestBody = new { enable };
            var content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );
            HttpResponseMessage response = await _client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Status code: {response.StatusCode}");
            }
        }
    }
}
