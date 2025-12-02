using ERP_API.Application.Interfaces.Inventory;
using ERP_API.Application.DTOs.Inventory.Packages;
using Microsoft.AspNetCore.Mvc;

namespace ERP_API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTypesController : ControllerBase
    {
        private readonly IPackageTypeService _packageTypeService;

        public PackageTypesController(IPackageTypeService packageTypeService)
        {
            _packageTypeService = packageTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _packageTypeService.GetAllPackageTypesAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PackageTypeInsertDto dto)
        {
            var createdPackageType = await _packageTypeService.AddPackageTypeAsync(dto);
            return Ok(createdPackageType);
        }

        [HttpGet("{id}/Products")]
        public async Task<IActionResult> GetPackageDetails(int id)
        {
            var result = await _packageTypeService.GetPackageDetailsWithProductsAsync(id);

            if (result == null)
            {
                return NotFound($"Package Type with ID {id} not found.");
            }

            return Ok(result);
        }
    }
}