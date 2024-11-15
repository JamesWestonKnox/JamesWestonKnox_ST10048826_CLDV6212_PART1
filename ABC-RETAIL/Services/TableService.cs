﻿//James Knox
//ST10048826
//GROUP 3
//References:
//OpenAI.2024.Chat-GPT (Version 3.5).[Large language model].Available at: https://chat.openai.com/ [Accessed: 28 September 2024]
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].
//McCall, B., 2024. CLDV_FunctionsApp. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_FunctionsApp.git [Accessed 30 September 2024].


using Azure;
using Azure.Data.Tables;
using ABC_RETAIL.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Data.SqlClient;


namespace ABC_RETAIL.Services
{
    public class TableService
    {
        //creates HttpClient
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        //creates variable to hold function URL
        private readonly string _functionUrl = "https://cldv-functions1.azurewebsites.net/api/StoreTableInfo";

        //Constructor for HttpClient
        public TableService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Uploads customerProfile to azure table storage using function
        /// </summary>
        /// <param name="profile">profile object</param>
        /// <returns></returns>
        public async Task AddEntityAsync(CustomerProfile profile)
        {

            //Serialize the CustomerProfile object to JSON
            var jsonContent = JsonSerializer.Serialize(profile);

            //Create content object
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            //send http request to function and await response
            var response = await _httpClient.PostAsync(_functionUrl, content);

            // Check if the response is successful
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error adding entity. Status Code: {response.StatusCode}, Content: {responseContent}");
            }
        }

        /// <summary>
        /// Inserts customer into sql database
        /// </summary>
        /// <param name="profile">Customer profile object</param>
        /// <returns></returns>
        public async Task InsertCustomerAsync(CustomerProfile profile)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var query = @"INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber) VALUES (@FirstName, @LastName, @Email, @PhoneNumber)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@LastName", profile.LastName);
                command.Parameters.AddWithValue("@Email", profile.Email);
                command.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
