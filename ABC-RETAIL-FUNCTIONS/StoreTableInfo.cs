using ABC_RETAIL.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ABC_RETAIL_FUNCTIONS
{
    public static class StoreTableInfo
    {
        [Function("StoreTableInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var profile = JsonSerializer.Deserialize<CustomerProfile>(requestBody);

            if (profile == null || string.IsNullOrEmpty(profile.PartitionKey) || string.IsNullOrEmpty(profile.RowKey))
            {
                return new BadRequestObjectResult("Table name, partition key, row key, and data must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("connection1");
            var serviceClient = new TableServiceClient(connectionString);
            var tableClient = serviceClient.GetTableClient(profile.PartitionKey);
            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity(profile.PartitionKey, profile.RowKey)
            {
                ["FirstName"] = profile.FirstName,
                ["LastName"] = profile.LastName,
                ["Email"] = profile.Email,
                ["PhoneNumber"] = profile.PhoneNumber
            };

            await tableClient.AddEntityAsync(entity);

            return new OkObjectResult("Data added to table");
        }
    }
}
