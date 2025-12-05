using ERP_MVC.Models.DTOs.Finance;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP_MVC.Services.Finance
{
    public class PaymentOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/PaymentOrder";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public PaymentOrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get All Payment Orders
        public async Task<List<PaymentOrderDto>> GetAllPaymentOrdersAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<List<PaymentOrderDto>>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<PaymentOrderDto>();
        }

        // Get Payment Order By ID
        public async Task<PaymentOrderDto?> GetPaymentOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<PaymentOrderDto>>(content, _jsonOptions);

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching payment order: {ex.Message}");
                return null;
            }
        }

        // Get Create Data (Customers / Suppliers / Accounts…)
        public async Task<CreatePaymentDataDto?> GetCreateDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/create-data");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<CreatePaymentDataDto>>(content, _jsonOptions);

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching create data: {ex.Message}");
                return null;
            }
        }

        // Create Payment Order
        public async Task<bool> CreatePaymentOrderAsync(CreatePaymentOrderDto dto, string? userId)
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
                Console.WriteLine($"Error creating payment order: {ex.Message}");
                return false;
            }
        }

        // Update Payment Order
        public async Task<bool> UpdatePaymentOrderAsync(int id, UpdatePaymentOrderDto dto)
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
                Console.WriteLine($"Error updating payment order: {ex.Message}");
                return false;
            }
        }

        // Delete Payment Order
        public async Task<bool> DeletePaymentOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting payment order: {ex.Message}");
                return false;
            }
        }

        // Helper API Response Class
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T Data { get; set; }
            public string Message { get; set; }
        }
    }
}
