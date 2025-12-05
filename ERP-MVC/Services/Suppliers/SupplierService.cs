using ERP_MVC.Models.DTOs.Suppliers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP_MVC.Services.Suppliers
{
    public class SupplierService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/Supplier";

        public SupplierService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get All Suppliers
        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<SupplierDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<SupplierDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching suppliers: {ex.Message}");
                return new List<SupplierDto>();
            }
        }

        // Get Supplier By ID
        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<SupplierDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching supplier: {ex.Message}");
                return null;
            }
        }

        // Get Supplier Details (with transactions)
        public async Task<SupplierDetailsDto?> GetSupplierDetailsAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}/details");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<SupplierDetailsDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching supplier details: {ex.Message}");
                return null;
            }
        }

        // Create Supplier
        public async Task<bool> CreateSupplierAsync(CreateSupplierDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating supplier: {ex.Message}");
                return false;
            }
        }

        // Update Supplier
        public async Task<bool> UpdateSupplierAsync(int id, UpdateSupplierDto dto)
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
                Console.WriteLine($"Error updating supplier: {ex.Message}");
                return false;
            }
        }

        // Delete Supplier
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting supplier: {ex.Message}");
                return false;
            }
        }

        // Get Supplier Transactions
        public async Task<List<SupplierTransactionDto>> GetSupplierTransactionsAsync(int supplierId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{supplierId}/transactions");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<SupplierTransactionDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<SupplierTransactionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transactions: {ex.Message}");
                return new List<SupplierTransactionDto>();
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
