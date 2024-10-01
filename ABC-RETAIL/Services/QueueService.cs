//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].



using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ABC_RETAIL.Services
{
    public class QueueService
    {
        //creates HttpClient
        private readonly HttpClient _httpClient;
        
        //creates variable to hold function URL
        private readonly string _functionUrl = "https://cldv-functions1.azurewebsites.net/api/ProcessQueueMessage";

        //Constructor for HttpClient
        public QueueService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Sends message to azure queue using function
        /// </summary>
        /// <param name="queueName">Name of desired queue</param>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        /// 
        public async Task SendMessageAsync(string queueName, string message)
        {
            //check if queue name is null or empty
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));
            }

            //check if message is null or empty
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }

            //create URL to call function,encoding the queuename and message
            var url = $"{_functionUrl}?queueName={Uri.EscapeDataString(queueName)}&message={Uri.EscapeDataString(message)}";

            //create http request message
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            ///send http request to function and await response
            var response = await _httpClient.SendAsync(request);

            //check if response is successfull
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error sending message to the queue via Azure Function. Status Code: {response.StatusCode}, Content: {responseContent}");
            }


        }
    }
}
