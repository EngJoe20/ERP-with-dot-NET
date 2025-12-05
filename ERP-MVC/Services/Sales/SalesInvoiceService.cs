using ERP_MVC.Models.DTOs.Sales;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ERP_MVC.Services.Sales
{
    public class SalesInvoiceService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesInvoiceService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<SalesInvoiceListItemDto>> GetAllInvoicesAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("SalesInvoices");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SalesInvoiceListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<SalesInvoiceListItemDto>();
            }

            return new List<SalesInvoiceListItemDto>();
        }

        public async Task<SalesInvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"SalesInvoices/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SalesInvoiceResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public async Task<bool> CreateInvoiceAsync(CreateSalesInvoiceDto dto)
        {
            AddAuthorizationHeader();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("SalesInvoices", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"SalesInvoices/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<SalesInvoiceListItemDto>> GetInvoicesByCustomerAsync(int customerId)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"SalesInvoices/Customer/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SalesInvoiceListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<SalesInvoiceListItemDto>();
            }

            return new List<SalesInvoiceListItemDto>();
        }
    }
}