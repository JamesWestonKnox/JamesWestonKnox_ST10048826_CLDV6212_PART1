//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].


using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace ABC_RETAIL.Services
{
    public class BlobService
    {
        //creates HttpClient
        private readonly HttpClient _httpClient;

        //creates variable to hold function URL
        private readonly string _functionUrl = "https://cldv-functions1.azurewebsites.net/api/UploadBlob?";

        //Constructor for HttpClient
        public BlobService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Uploads blob to azure blob storage using function
        /// </summary>
        /// <param name="containerName">Name of desired storageContainer</param>
        /// <param name="blobName">blobName to upload</param>
        /// <returns></returns>
        /// 
        public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
        {
            // Create the URL for the function call, encoding containerName and blobName
            var url = $"{_functionUrl}containerName={Uri.EscapeDataString(containerName)}&blobName={Uri.EscapeDataString(blobName)}";

            //create instance of MultiFormDataContent to hold request body
            using var formContent = new MultipartFormDataContent();

            //create streamContent object
            var streamContent = new StreamContent(content);

            //indicate content type
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            
            formContent.Add(streamContent, "file", blobName);

            //send http request to function and await response
            var response = await _httpClient.PostAsync(url, formContent);

            //check if response is successfull
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error uploading blob. Status Code: {response.StatusCode}, Content: {responseContent}");
            }
        }

    }
}
