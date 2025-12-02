using ERP_API.Application.Interfaces;
using ERP_API.Application.DTOs.Inventory.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ERP_API.Application.Interfaces.Inventory;

namespace ERP_API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInsertDto productDto)
        {
            var createdProduct = await _productService.AddProductAsync(productDto);
            return Ok(createdProduct);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.GetAllProductsAsync());
        }

        [HttpPost("{productId}/Variations")]
        public async Task<IActionResult> AddVariation(int productId, VariationInsertDto dto)
        {
            var result = await _productService.AddVariationAsync(productId, dto);
            return Ok(result);
        }

        [HttpPost("Variations/{variationId}/Packages")]
        public async Task<IActionResult> AddPackage(int variationId, PackageLinkInsertDto dto)
        {
            var result = await _productService.AddPackageAsync(variationId, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(product);
        }
    }
}
