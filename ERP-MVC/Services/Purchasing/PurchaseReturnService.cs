using ERP_MVC.Models.DTOs.Purchasing;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ERP_MVC.Services.Purchasing
{
    public class PurchaseReturnService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseReturnService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri("https://localhost:7052/api/");
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirstValue("jwt");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<PurchaseReturnListItemDto>> GetAllReturnsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("PurchaseReturns");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PurchaseReturnListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<PurchaseReturnListItemDto>();
            }

            return new List<PurchaseReturnListItemDto>();
        }

        public async Task<PurchaseReturnResponseDto?> GetReturnByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"PurchaseReturns/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PurchaseReturnResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public async Task<bool> CreateReturnAsync(CreatePurchaseReturnDto dto)
        {
            AddAuthorizationHeader();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("PurchaseReturns", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReturnAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"PurchaseReturns/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PurchaseReturnListItemDto>> GetReturnsBySupplierAsync(int supplierId)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"PurchaseReturns/Supplier/{supplierId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PurchaseReturnListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<PurchaseReturnListItemDto>();
            }

            return new List<PurchaseReturnListItemDto>();
        }
    }
}