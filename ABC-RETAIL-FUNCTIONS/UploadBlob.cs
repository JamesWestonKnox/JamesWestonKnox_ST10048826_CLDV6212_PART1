using Azure.Storage.Blobs;
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
    public static class UploadBlob
    {
        /// <summary>
        /// Function to upload blob to Azure Blob Storage
        /// </summary>
        /// <param name="req">Http request containing blob info</param>
        /// <returns>Return either bad or ok object result depending on success or failure</returns>
        [Function("UploadBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            //sets containerName to containerName parameter in http request
            string containerName = req.Query["containerName"];

            //sets blobName to blobName parameter in http request
            string blobName = req.Query["blobName"];

            //checks both strings are not null or empty and returns bad object result if they are.
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                return new BadRequestObjectResult("Container name and blob name must be provided.");
            }

            //connects function to azure storage account through connection stored in function app enviromental varaibles
            var connectionString = Environment.GetEnvironmentVariable("connection1");

            //creates new blob service using connection string
            var blobServiceClient = new BlobServiceClient(connectionString);

            //looks for container matching container name
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            //creates container if existing not found
            await containerClient.CreateIfNotExistsAsync();

            //sets blobClient to specific blob
            var blobClient = containerClient.GetBlobClient(blobName);

            //uses request body as stream to upload blob
            using var stream = req.Body;

            //Uploads blob
            await blobClient.UploadAsync(stream, true);

            return new OkObjectResult("Blob uploaded");
        }
    }
}
