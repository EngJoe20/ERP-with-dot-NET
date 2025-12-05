using ERP_MVC.Models.DTOs.Finance;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP_MVC.Services.Finance
{
    public class MainSafeLedgerEntryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/MainSafeLedgerEntry";

        public MainSafeLedgerEntryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get All Ledger Entries
        public async Task<List<MainSafeLedgerEntryDto>> GetAllLedgerEntriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MainSafeLedgerEntryDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<MainSafeLedgerEntryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ledger entries: {ex.Message}");
                return new List<MainSafeLedgerEntryDto>();
            }
        }

        // Get Ledger Entry By ID
        public async Task<MainSafeLedgerEntryDto?> GetLedgerEntryByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<MainSafeLedgerEntryDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ledger entry: {ex.Message}");
                return null;
            }
        }

        // Get Ledger Entry Details
        public async Task<MainSafeLedgerEntryDetailsDto?> GetLedgerEntryDetailsAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}/details");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<MainSafeLedgerEntryDetailsDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ledger entry details: {ex.Message}");
                return null;
            }
        }

        // Get Ledger Entries By Main Safe ID
        public async Task<List<MainSafeLedgerEntryDto>> GetLedgerEntriesByMainSafeIdAsync(int mainSafeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/main-safe/{mainSafeId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MainSafeLedgerEntryDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<MainSafeLedgerEntryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ledger entries by main safe: {ex.Message}");
                return new List<MainSafeLedgerEntryDto>();
            }
        }

        // Get Filtered Ledger Entries
        public async Task<List<MainSafeLedgerEntryDto>> GetFilteredLedgerEntriesAsync(MainSafeLedgerEntryFilterDto filter)
        {
            try
            {
                var json = JsonSerializer.Serialize(filter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/filter", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MainSafeLedgerEntryDto>>>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<MainSafeLedgerEntryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching filtered ledger entries: {ex.Message}");
                return new List<MainSafeLedgerEntryDto>();
            }
        }

        // Get Ledger Entries By Date Range
        public async Task<List<MainSafeLedgerEntryDto>> GetLedgerEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MainSafeLedgerEntryDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<MainSafeLedgerEntryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ledger entries by date range: {ex.Message}");
                return new List<MainSafeLedgerEntryDto>();
            }
        }

        // Get Ledger Summary
        public async Task<MainSafeLedgerSummaryDto?> GetLedgerSummaryAsync(int mainSafeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{_baseUrl}/summary/{mainSafeId}";
                if (startDate.HasValue)
                    url += $"?startDate={startDate.Value:yyyy-MM-dd}";
                if (endDate.HasValue)
                    url += startDate.HasValue ? $"&endDate={endDate.Value:yyyy-MM-dd}" : $"?endDate={endDate.Value:yyyy-MM-dd}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<MainSafeLedgerSummaryDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ledger summary: {ex.Message}");
                return null;
            }
        }

        // Get Latest Entries
        public async Task<List<MainSafeLedgerEntryDto>> GetLatestEntriesAsync(int mainSafeId, int count = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/latest/{mainSafeId}?count={count}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MainSafeLedgerEntryDto>>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data ?? new List<MainSafeLedgerEntryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching latest entries: {ex.Message}");
                return new List<MainSafeLedgerEntryDto>();
            }
        }

        // Create Ledger Entry
        public async Task<bool> CreateLedgerEntryAsync(CreateMainSafeLedgerEntryDto dto)
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
                Console.WriteLine($"Error creating ledger entry: {ex.Message}");
                return false;
            }
        }

        // Update Ledger Entry
        public async Task<bool> UpdateLedgerEntryAsync(int id, UpdateMainSafeLedgerEntryDto dto)
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
                Console.WriteLine($"Error updating ledger entry: {ex.Message}");
                return false;
            }
        }

        // Delete Ledger Entry
        public async Task<bool> DeleteLedgerEntryAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting ledger entry: {ex.Message}");
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