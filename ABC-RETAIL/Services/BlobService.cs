﻿using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace ABC_RETAIL.Services
{
    public class BlobService
    {
        private readonly HttpClient _httpClient;

        private readonly string _functionUrl = " https://cldv-functions1.azurewebsites.net/api/UploadBlob?";

        public BlobService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
        {
            // Create the URL for the function call
            var url = $"{_functionUrl}containerName={Uri.EscapeDataString(containerName)}&blobName={Uri.EscapeDataString(blobName)}";

            using var formContent = new MultipartFormDataContent();


            var streamContent = new StreamContent(content);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            formContent.Add(streamContent, "file", blobName);

            // Call the Azure Function
            var response = await _httpClient.PostAsync(url, formContent);

            // Check the response status
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error uploading blob. Status Code: {response.StatusCode}, Content: {responseContent}");
            }
        }

    }
}
