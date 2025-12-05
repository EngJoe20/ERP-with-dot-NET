using ERP_MVC.Models.DTOs.Finance;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ERP_MVC.Services.Finance
{
    public class ReceiptOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/ReceiptOrder";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ReceiptOrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get All Receipt Orders
        //public async Task<List<ReceiptOrderDto>> GetAllReceiptOrdersAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync(_baseUrl);
        //        response.EnsureSuccessStatusCode();

        //        var content = await response.Content.ReadAsStringAsync();
        //        var result = JsonSerializer.Deserialize<ApiResponse<List<ReceiptOrderDto>>>(content,
        //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //        return result?.Data ?? new List<ReceiptOrderDto>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error fetching ledger entries: {ex.Message}");
        //        return new List<ReceiptOrderDto>();
        //    }
        //}
        public async Task<List<ReceiptOrderDto>> GetAllReceiptOrdersAsync()
        {
            var response = await _httpClient.GetAsync("api/ReceiptOrder");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<List<ReceiptOrderDto>>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<ReceiptOrderDto>();
        }


        // Get Receipt Order By ID
        public async Task<ReceiptOrderDto?> GetReceiptOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<ReceiptOrderDto>>(content, _jsonOptions);

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching receipt order: {ex.Message}");
                return null;
            }
        }

        // Get Create Data (Customers and Suppliers)
        public async Task<CreateDataDto?> GetCreateDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/create-data");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<CreateDataDto>>(content, _jsonOptions);

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching create data: {ex.Message}");
                return null;
            }
        }

        // Create Receipt Order
        public async Task<bool> CreateReceiptOrderAsync(CreateReceiptOrderDto dto, string? userId)
        {
            try
            {
                dto.PerformedByUserId = userId ?? string.Empty;

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating receipt order: {ex.Message}");
                return false;
            }
        }

        // Update Receipt Order
        public async Task<bool> UpdateReceiptOrderAsync(int id, UpdateReceiptOrderDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating receipt order: {ex.Message}");
                return false;
            }
        }

        // Delete Receipt Order
        public async Task<bool> DeleteReceiptOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting receipt order: {ex.Message}");
                return false;
            }
        }

        // Helper class for API responses
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T Data { get; set; }
            public string Message { get; set; }
        }
    }
}
