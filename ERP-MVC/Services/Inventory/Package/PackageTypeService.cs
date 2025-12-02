using ERP_MVC.Models.DTOs.Inventory.Packages;


namespace ERP_MVC.Services.Inventory.Package
{
    public class PackageTypeService
    {
        private readonly HttpClient _httpClient;

        public PackageTypeService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            
            string baseUrl = config["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        
        public async Task<List<PackageTypeItemDto>> GetAllPackageTypes()
        {
            var result = await _httpClient.GetFromJsonAsync<List<PackageTypeItemDto>>("api/PackageTypes");
            return result ?? new List<PackageTypeItemDto>();
        }

       
        public async Task<bool> CreatePackageType(PackageTypeInsertDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PackageTypes", dto);
            return response.IsSuccessStatusCode;
        }

     
        public async Task<PackageDetailsDto?> GetPackageDetailsWithProducts(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PackageDetailsDto>($"api/PackageTypes/{id}/Products");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // Handle 404 gracefully
            }
        }
    }
}
