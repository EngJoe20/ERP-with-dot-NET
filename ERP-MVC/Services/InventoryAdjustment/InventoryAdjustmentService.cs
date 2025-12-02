using ERP_MVC.Models.DTOs.InventoryAdjustment;


namespace ERP_MVC.Services.InventoryAdjustment
{
    public class InventoryAdjustmentService
    {
        private readonly HttpClient _httpClient;

        public InventoryAdjustmentService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            string baseUrl = config["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        // 1. Create Adjustment (POST)
        // Consumes: POST api/InventoryAdjustment/Adjustments
        // ✅ FIX: Accept DTO directly, not ViewModel
        public async Task<bool> CreateAdjustmentAsync(CreateAdjustmentDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/InventoryAdjustment/Adjustments", dto);
            return response.IsSuccessStatusCode;
        }

        // 2. Get Logs (GET)
        // Consumes: GET api/InventoryAdjustment/Adjustments
        public async Task<List<AdjustmentLogDto>> GetAdjustmentLogsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<AdjustmentLogDto>>("api/InventoryAdjustment/Adjustments");
            return result ?? new List<AdjustmentLogDto>();
        }
    }
}
