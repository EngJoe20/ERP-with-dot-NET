using ERP_MVC.Models.DTOs.Warehouse;


namespace ERP_MVC.Services.Warehouse
{
    public class WarehouseService
    {
        private readonly HttpClient _httpClient;

        public WarehouseService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            string baseUrl = config["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        // 1. Get All Warehouses (For Dropdowns & List Page)
        // Consumes: GET api/Warehouses
        public async Task<List<WarehouseItemDto>> GetAllWarehouses()
        {
            var result = await _httpClient.GetFromJsonAsync<List<WarehouseItemDto>>("api/Warehouses");
            return result ?? new List<WarehouseItemDto>();
        }

        // 2. Create Warehouse
        // Consumes: POST api/Warehouses
        public async Task<bool> CreateWarehouse(WarehouseInsertDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Warehouses", dto);
            return response.IsSuccessStatusCode;
        }

        // 3. Transfer Stock
        // Consumes: POST api/Warehouses/Transfer
        public async Task<bool> TransferStock(StockTransferDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Warehouses/Transfer", dto);
            return response.IsSuccessStatusCode;
        }

        // 4. Get Specific Stock List
        // Consumes: GET api/Warehouses/{id}/Stock
        public async Task<List<WarehouseStockDto>> GetWarehouseStock(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<WarehouseStockDto>>($"api/Warehouses/{id}/Stock");
            return result ?? new List<WarehouseStockDto>();
        }

        // 5. Get Transfer Logs
        // Consumes: GET api/Warehouses/Logs
        public async Task<List<StockTransferLogDto>> GetTransferLogs()
        {
            var result = await _httpClient.GetFromJsonAsync<List<StockTransferLogDto>>("api/Warehouses/Logs");
            return result ?? new List<StockTransferLogDto>();
        }
    }
}
