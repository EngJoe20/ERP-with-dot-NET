using ERP_MVC.Models.DTOs.Inventory.Product;
using ERP_MVC.Services.Inventory.Package;
using ERP_MVC.Services.Inventory.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_MVC.Controllers.Product
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly PackageTypeService _packageService;

        // Inject BOTH services
        public ProductController(ProductService productService, PackageTypeService packageService)
        {
            _productService = productService;
            _packageService = packageService;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts();
            return View(products);
            // MVC looks for: Views/Products/Index.cshtml
        }


        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
            // MVC looks for: Views/Products/Details.cshtml
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var packages = await _packageService.GetAllPackageTypes();

            ViewBag.PackageList = packages.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            return View(new ProductInsertDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductInsertDto dto)
        {
            // Note: In a real app, check ModelState.IsValid here

            // Call API
            bool success = await _productService.CreateProductAsync(dto);

            if (success)
            {
                return RedirectToAction("Index"); // Go back to list on success
            }

            // IF FAILURE: Reload the dropdowns and show form again
            var packages = await _packageService.GetAllPackageTypes();
            ViewBag.PackageList = packages.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            ModelState.AddModelError("", "Error saving product. Please try again.");
            return View(dto);
        }


        [HttpGet]
        public async Task<IActionResult> AddVariation(int productId)
        {
            // 1. Get Dropdown Data
            var packages = await _packageService.GetAllPackageTypes();

            // 2. Pass Dropdown via ViewBag
            ViewBag.PackageTypeOptions = packages.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            // 3. Pass ProductId via ViewBag (or a TempData/hidden field strategy)
            ViewBag.ProductId = productId;

            // 4. Return Empty DTO
            return View(new VariationInsertDto());
        }


        [HttpPost]
        public async Task<IActionResult> AddVariation(int productId, VariationInsertDto dto)
        {
            // 1. Call API via Service
            bool success = await _productService.AddVariationAsync(productId, dto);

            if (success)
            {
                TempData["SuccessMessage"] = "New variation has been added successfully!";

                // Redirect back to the Product Details page
                return RedirectToAction("ProductDetails", new { id = productId });
            }

            var packages = await _packageService.GetAllPackageTypes();
            ViewBag.PackageTypeOptions = packages.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            // Restore ProductId
            ViewBag.ProductId = productId;

            ModelState.AddModelError("", "Failed to add variation. Please check your inputs.");
            return View(dto);
        }


        [HttpGet]
        public async Task<IActionResult> AddPackage(int variationId, int productId)
        {
            // 1. Get Dropdown Data
            var packages = await _packageService.GetAllPackageTypes();

            // 2. Pass Dropdown via ViewBag
            ViewBag.PackageTypeOptions = packages.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            // 3. Pass both IDs to keep them for the POST and for navigation
            ViewBag.VariationId = variationId;
            ViewBag.ProductId = productId;

            return View(new PackageLinkInsertDto());
        }


        [HttpPost]
        public async Task<IActionResult> AddPackage(int variationId, int productId, PackageLinkInsertDto dto)
        {
            // 1. Call API
            bool success = await _productService.AddPackageAsync(variationId, dto);

            if (success)
            {
                TempData["SuccessMessage"] = "New package added successfully!";

                // Redirect back to ProductDetails with the productId we already have
                return RedirectToAction("ProductDetails", new { id = productId });
            }

            // Failure: Reload Dropdown
            var packages = await _packageService.GetAllPackageTypes();
            ViewBag.PackageTypeOptions = packages.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            ViewBag.VariationId = variationId;
            ViewBag.ProductId = productId;

            ModelState.AddModelError("", "Failed to add package.");
            return View(dto);
        }
    }
}
