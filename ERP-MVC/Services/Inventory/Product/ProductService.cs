using ERP_MVC.Models.DTOs.Inventory.Product;
using ERP_MVC.Models.DTOs.Inventory.Product.Responses;
using ERP_MVC.Models.DTOs.Inventory.Packages;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;


namespace ERP_MVC.Services.Inventory.Product
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            string baseUrl = config["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        // 1. Get Details
        public async Task<ProductResponseDto?> GetProductById(int id)
        {
            try
            {
                // Matches API: GET api/Products/{id}
                return await _httpClient.GetFromJsonAsync<ProductResponseDto>($"api/Products/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        // 2. Get All
        public async Task<List<ProductSummaryDto>> GetAllProducts()
        {
            // Matches API: GET api/Products
            var result = await _httpClient.GetFromJsonAsync<List<ProductSummaryDto>>("api/Products");
            return result ?? new List<ProductSummaryDto>();
        }

        // 3. Get Package Types (Helper for Dropdown)
        public async Task<IEnumerable<PackageTypeItemDto>> GetPackageTypesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<PackageTypeItemDto>>("api/PackageTypes");
            return result ?? new List<PackageTypeItemDto>();
        }

        // 4. Create Product
        public async Task<bool> CreateProductAsync(ProductInsertDto dto)
        {
            // No mapping needed! Just pass it through.
            var response = await _httpClient.PostAsJsonAsync("api/Products", dto);
            return response.IsSuccessStatusCode;
        }

    
        public async Task<bool> AddVariationAsync(int productId, VariationInsertDto dto)
        {
            // Matches API: POST api/Products/{id}/Variations
            var response = await _httpClient.PostAsJsonAsync($"api/Products/{productId}/Variations", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddPackageAsync(int variationId, PackageLinkInsertDto dto)
        {
            // Matches API: POST api/Products/Variations/{id}/Packages
            var response = await _httpClient.PostAsJsonAsync($"api/Products/Variations/{variationId}/Packages", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
