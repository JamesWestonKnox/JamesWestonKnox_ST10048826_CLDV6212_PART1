//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].


using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC_RETAIL_FUNCTIONS
{
    public static class ProcessQueueMessage
    {
        /// <summary>
        /// Function to receive information from QueueService and add message to the queue.
        /// </summary>
        /// <param name="req">Http request containing queue message data</param>
        /// <returns>Return either bad or ok object result depending on success or failure</returns>
        [Function("ProcessQueueMessage")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            //http request for queuename
            string queueName = req.Query["queueName"];

            //http request for queue message
            string message = req.Query["message"];

            //checks if either queueName or message are null or empty and return error message if true
            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            //connects to storage account using connection defined in function app enviroment variables.
            var connectionString = Environment.GetEnvironmentVariable("connection1");

            //creates new Queue Service
            var queueServiceClient = new QueueServiceClient(connectionString);

            //finds queue matching queue name
            var queueClient = queueServiceClient.GetQueueClient(queueName);

            //creates queue with queue name if one does not exist
            await queueClient.CreateIfNotExistsAsync();

            //sends message to queue
            await queueClient.SendMessageAsync(message);

            return new OkObjectResult("Message added to queue");
        }
    }
}
