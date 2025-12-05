using ERP_MVC.Models.DTOs.Customers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP_MVC.Services.Customers
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/Customer";

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get All Customers
        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<CustomerDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<CustomerDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customers: {ex.Message}");
                return new List<CustomerDto>();
            }
        }

        // Get Customer By ID
        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<CustomerDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer: {ex.Message}");
                return null;
            }
        }

        // Get Customer Details (with transactions)
        public async Task<CustomerDetailsDto?> GetCustomerDetailsAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}/details");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<CustomerDetailsDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer details: {ex.Message}");
                return null;
            }
        }

        // Create Customer
        public async Task<bool> CreateCustomerAsync(CreateCustomerDto dto)
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
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return false;
            }
        }

        // Update Customer
        public async Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
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
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        // Delete Customer
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }

        // Get Customer Transactions
        public async Task<List<CustomerTransactionDto>> GetCustomerTransactionsAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{customerId}/transactions");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<CustomerTransactionDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<CustomerTransactionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transactions: {ex.Message}");
                return new List<CustomerTransactionDto>();
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
