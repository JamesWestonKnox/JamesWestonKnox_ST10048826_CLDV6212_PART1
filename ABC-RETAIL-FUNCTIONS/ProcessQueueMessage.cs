﻿using Azure.Storage.Queues;
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
        [Function("ProcessQueueMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string queueName = req.Query["queueName"];
            string message = req.Query["message"];

            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                log.LogError("Queue name or message is null or empty.");
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("connection1");
            var queueServiceClient = new QueueServiceClient(connectionString);
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);

            return new OkObjectResult("Message added to queue");
        }
    }
}
