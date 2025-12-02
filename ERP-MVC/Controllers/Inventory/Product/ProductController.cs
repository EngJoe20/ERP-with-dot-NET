using Microsoft.AspNetCore.Mvc;
using ERP_MVC.Services.Inventory.Product;

namespace ERP_MVC.Controllers
{
    public class ProductController : Controller
    {
        // We inject the service we created earlier
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: /Product/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. Call the API via the Service
            var products = await _productService.GetAllProducts();

            // 2. Pass the list to the View
            return View("~/Views/Inventory/Product/Index.cshtml", products);
        }
    }
}
