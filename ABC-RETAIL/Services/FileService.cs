//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].


using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ABC_RETAIL.Services
{
    public class FileService
    {
        //creates HttpClient
        private readonly HttpClient _httpClient;

        //creates variable to hold function URL
        private readonly string _functionUrl = " https://cldv-functions1.azurewebsites.net/api/UploadFile?";

        //Constructor for HttpClient
        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Uploads fileShare to azure fileShare using function
        /// </summary>
        /// <param name="shareName">Name of fileShareContainer</param>
        /// <param name="fileName">Name of file</param>
        /// <returns></returns>
        public async Task UploadFileAsync(string shareName, string fileName, Stream content)
        {
            // Create the URL for the function call, encoding shareName and fileName
            var url = $"{_functionUrl}shareName={Uri.EscapeDataString(shareName)}&fileName={Uri.EscapeDataString(fileName)}";

            //create instance of MultiFormDataContent to hold request body
            using var formContent = new MultipartFormDataContent();

            //create streamContent object
            var streamContent = new StreamContent(content);

            //indicate content type
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            
            formContent.Add(streamContent, "file", fileName);

            //send http request to function and await response
            var response = await _httpClient.PostAsync(url, formContent);

            //check if response is successfull
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error uploading file. Status Code: {response.StatusCode}, Content: {responseContent}");
            }
        }
    }
}
