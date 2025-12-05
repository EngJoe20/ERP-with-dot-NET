using ERP_MVC.Models.DTOs.Purchasing;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ERP_MVC.Services.Purchasing
{
    public class PurchaseInvoiceService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseInvoiceService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<PurchaseInvoiceListItemDto>> GetAllInvoicesAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("PurchaseInvoices");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PurchaseInvoiceListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<PurchaseInvoiceListItemDto>();
            }

            return new List<PurchaseInvoiceListItemDto>();
        }

        public async Task<PurchaseInvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"PurchaseInvoices/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PurchaseInvoiceResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public async Task<bool> CreateInvoiceAsync(CreatePurchaseInvoiceDto dto)
        {
            AddAuthorizationHeader();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("PurchaseInvoices", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"PurchaseInvoices/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PurchaseInvoiceListItemDto>> GetInvoicesBySupplierAsync(int supplierId)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"PurchaseInvoices/Supplier/{supplierId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PurchaseInvoiceListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<PurchaseInvoiceListItemDto>();
            }

            return new List<PurchaseInvoiceListItemDto>();
        }
    }
}