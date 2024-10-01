﻿//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].


using Azure;
using Azure.Data.Tables;
using ABC_RETAIL.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text;


namespace ABC_RETAIL.Services
{
    public class TableService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QueueService> _logger;

        private readonly string _functionUrl = "https://cldv-functions1.azurewebsites.net/api/StoreTableInfo";

        public TableService(HttpClient httpClient, ILogger<QueueService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddEntityAsync(CustomerProfile profile)
        {
            try
            {
                // Serialize the CustomerProfile object to JSON
                var jsonContent = JsonSerializer.Serialize(profile);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Call the Azure Function
                var response = await _httpClient.PostAsync(_functionUrl, content);

                // Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error adding entity. Status Code: {response.StatusCode}, Content: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity to table");
                throw; // Rethrow after logging
            }
        }
    }
}
