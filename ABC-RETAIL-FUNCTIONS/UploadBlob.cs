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
        [Function("UploadBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            string containerName = req.Query["containerName"];
            string blobName = req.Query["blobName"];

            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                return new BadRequestObjectResult("Container name and blob name must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("connection1");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!req.ContentType.StartsWith("multipart/form-data"))
            {
                return new BadRequestObjectResult("Request must be multipart/form-data.");
            }

            // Read the file from the request
            var formCollection = await req.ReadFormAsync();
            var file = formCollection.Files[0];

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return new OkObjectResult("Blob uploaded");
        }
    }
}
