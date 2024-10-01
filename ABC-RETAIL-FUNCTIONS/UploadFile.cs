//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].

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
            if (!req.ContentType.StartsWith("multipart/form-data"))
            {
                return new BadRequestObjectResult("Request must be multipart/form-data.");
            }

            //read request data
            var formCollection = await req.ReadFormAsync();

            var file = formCollection.Files[0];

            //create same length file in file share
            await fileClient.CreateAsync(file.Length);
            
            //read file contents
            using var stream = file.OpenReadStream();

            //upload file to azure file share
            await fileClient.UploadAsync(stream);

            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}
