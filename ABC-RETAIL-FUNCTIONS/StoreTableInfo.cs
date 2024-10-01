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
        /// <summary>
        /// Function to store customer profile into Azure Table Storage
        /// </summary>
        /// <param name="req">HTTP request with customer data</param>
        /// <returns>Return either bad or ok object result depending on success or failure</returns>
        [Function("StoreTableInfo")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {

            //sets requestBody to the content of the http request
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            //Deserialise requestBody variable into a customer profile object
            var profile = JsonSerializer.Deserialize<CustomerProfile>(requestBody);

            //checks whether profile object is null and that partition and row keys are valid
            if (profile == null || string.IsNullOrEmpty(profile.PartitionKey) || string.IsNullOrEmpty(profile.RowKey))
            {
                //return error message for bad request
                return new BadRequestObjectResult("Table name, partition key, row key, and data must be provided.");
            }

            //Connects function to azure storage account through connection stored in function app enviromental varaibles
            var connectionString = Environment.GetEnvironmentVariable("connection1");

            //Creates new table service using connection string
            var serviceClient = new TableServiceClient(connectionString);

            //Looks for table associated with partition key
            var tableClient = serviceClient.GetTableClient(profile.PartitionKey);

            //creates new table if partition key does not match existing table
            await tableClient.CreateIfNotExistsAsync();

            //creats table entity and gives it properties from profile object
            var entity = new TableEntity(profile.PartitionKey, profile.RowKey)
            {
                ["FirstName"] = profile.FirstName,
                ["LastName"] = profile.LastName,
                ["Email"] = profile.Email,
                ["PhoneNumber"] = profile.PhoneNumber
            };

            //adds entity to table
            await tableClient.AddEntityAsync(entity);

            return new OkObjectResult("Data added to table");
        }
    }
}
