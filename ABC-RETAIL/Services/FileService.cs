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
        private readonly HttpClient _httpClient;

        private readonly string _functionUrl = " https://cldv-functions1.azurewebsites.net/api/UploadFile?";

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task UploadFileAsync(string shareName, string fileName, Stream content)
        {
            // Create the URL for the function call
            var url = $"{_functionUrl}shareName={Uri.EscapeDataString(shareName)}&fileName={Uri.EscapeDataString(fileName)}";

            using var formContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(content);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            formContent.Add(streamContent, "file", fileName);

            // Call the Azure Function
            var response = await _httpClient.PostAsync(url, formContent);

            // Check the response status
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error uploading file. Status Code: {response.StatusCode}, Content: {responseContent}");
            }
        }
    }
}
