using ERP_MVC.Models.DTOs.Sales;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ERP_MVC.Services.Sales
{
    public class SalesReturnService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesReturnService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<SalesReturnListItemDto>> GetAllReturnsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("SalesReturns");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SalesReturnListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<SalesReturnListItemDto>();
            }

            return new List<SalesReturnListItemDto>();
        }

        public async Task<SalesReturnResponseDto?> GetReturnByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"SalesReturns/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SalesReturnResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public async Task<bool> CreateReturnAsync(CreateSalesReturnDto dto)
        {
            AddAuthorizationHeader();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("SalesReturns", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReturnAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"SalesReturns/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<SalesReturnListItemDto>> GetReturnsByCustomerAsync(int customerId)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"SalesReturns/Customer/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SalesReturnListItemDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<SalesReturnListItemDto>();
            }

            return new List<SalesReturnListItemDto>();
        }
    }
}