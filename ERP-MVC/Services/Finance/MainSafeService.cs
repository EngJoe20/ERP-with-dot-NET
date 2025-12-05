using ERP_MVC.Models.DTOs.Finance;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP_MVC.Services.Finance
{
    public class MainSafeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/MainSafe";

        public MainSafeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get Main Safe Statement
        public async Task<MainSafeDto?> GetStatementAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/statement");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<MainSafeDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching main safe statement: {ex.Message}");
                return null;
            }
        }

        // Get All Main Safes
        public async Task<List<MainSafeDto>> GetAllMainSafesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MainSafeDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<MainSafeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching main safes: {ex.Message}");
                return new List<MainSafeDto>();
            }
        }

        // Get Main Safe By ID
        public async Task<MainSafeDto?> GetMainSafeByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<MainSafeDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching main safe: {ex.Message}");
                return null;
            }
        }

        // Create Main Safe
        public async Task<bool> CreateMainSafeAsync(CreateMainSafeDto dto)
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
                Console.WriteLine($"Error creating main safe: {ex.Message}");
                return false;
            }
        }

        // Update Main Safe
        public async Task<bool> UpdateMainSafeAsync(int id, UpdateMainSafeDto dto)
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
                Console.WriteLine($"Error updating main safe: {ex.Message}");
                return false;
            }
        }

        // Delete Main Safe
        public async Task<bool> DeleteMainSafeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting main safe: {ex.Message}");
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