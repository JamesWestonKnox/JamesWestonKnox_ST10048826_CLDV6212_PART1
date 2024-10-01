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
        /// <summary>
        /// Function to upload file to Azure file share service
        /// </summary>
        /// <param name="req">Http request containg file data</param>
        /// <returns>Return either bad or ok object result depending on success or failure</returns>
        [Function("UploadFile")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            //sets shareName to shareName parameter in http request
            string shareName = req.Query["shareName"];

            //sets fileName to fileName parameter in http request
            string fileName = req.Query["fileName"];

            //checks both strings are not null or empty and returns bad object result if they are.
            if (string.IsNullOrEmpty(shareName) || string.IsNullOrEmpty(fileName))
            {
                return new BadRequestObjectResult("Share name and file name must be provided.");
            }

            //Connects function to azure storage account through connection stored in function app enviromental varaibles
            var connectionString = Environment.GetEnvironmentVariable("connection1");

            //creates new fileShare service using connection string
            var shareServiceClient = new ShareServiceClient(connectionString);

            //looks for fileShare matching shareName
            var shareClient = shareServiceClient.GetShareClient(shareName);

            //creates new fileShare if it doesnt exist
            await shareClient.CreateIfNotExistsAsync();

            //finds root directory of fileShare client
            var directoryClient = shareClient.GetRootDirectoryClient();

            //finds specific fileShare file
            var fileClient = directoryClient.GetFileClient(fileName);

            //reads file stream
            using var stream = req.Body;

            //creates new file same length as filestream
            await fileClient.CreateAsync(stream.Length);

            //uploads file
            await fileClient.UploadAsync(stream);

            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}
