using Azure.Storage.Files.Shares;
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
    public static class UploadFile
    {
        [Function("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            string shareName = req.Query["shareName"];
            string fileName = req.Query["fileName"];

            if (string.IsNullOrEmpty(shareName) || string.IsNullOrEmpty(fileName))
            {
                return new BadRequestObjectResult("Share name and file name must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("connection1");
            var shareServiceClient = new ShareServiceClient(connectionString);
            var shareClient = shareServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);

            if (!req.ContentType.StartsWith("multipart/form-data"))
            {
                return new BadRequestObjectResult("Request must be multipart/form-data.");
            }

            var formCollection = await req.ReadFormAsync();
            var file = formCollection.Files[0];

            await fileClient.CreateAsync(file.Length);
            using var stream = file.OpenReadStream();
            await fileClient.UploadAsync(stream);

            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}
