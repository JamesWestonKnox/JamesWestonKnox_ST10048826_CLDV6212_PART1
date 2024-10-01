using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ABC_RETAIL.Services
{
    public class QueueService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QueueService> _logger;

        private readonly string _functionUrl = " https://cldv-functions1.azurewebsites.net/api/ProcessQueueMessage";
        public QueueService(HttpClient httpClient, ILogger<QueueService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task SendMessageAsync(string queueName, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(queueName))
                {
                    throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));
                }

                if (string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException("Message cannot be null or empty.", nameof(message));
                }

                // Constructing the URL with the query parameters
                var url = $"{_functionUrl}?queueName={Uri.EscapeDataString(queueName)}&message={Uri.EscapeDataString(message)}";

                // Creating the POST request to the Azure Function
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                // Sending the request
                var response = await _httpClient.SendAsync(request);

                // Checking the response status and throwing an error if it fails
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error sending message to the queue via Azure Function. Status Code: {response.StatusCode}, Content: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to the queue");
                throw; 
            }
        }
    }
}
