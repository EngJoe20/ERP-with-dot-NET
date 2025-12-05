using ERP_MVC.Models.DTOs.Warehouse;
using ERP_MVC.Services.Inventory.Product;
using ERP_MVC.Services.Warehouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_MVC.Controllers.Warehouse
{
    public class WarehouseController : Controller
    {
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService; // Needed for Transfer logic

        public WarehouseController(WarehouseService warehouseService, ProductService productService)
        {
            _warehouseService = warehouseService;
            _productService = productService;
        }

       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseService.GetAllWarehouses();
            return View(warehouses);
        }

       
        [HttpGet]
        public IActionResult Create()
        {
            return View(new WarehouseInsertDto());
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(WarehouseInsertDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            bool success = await _warehouseService.CreateWarehouse(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Warehouse created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to create warehouse.");
            return View(dto);
        }

       
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
           
            var allWarehouses = await _warehouseService.GetAllWarehouses();
            var warehouse = allWarehouses.FirstOrDefault(w => w.Id == id);

            if (warehouse == null) return NotFound();

            // 2. Get Stock Items
            var stockItems = await _warehouseService.GetWarehouseStock(id);

            // 3. Build ViewModel (or reuse DTO)
            var viewModel = new WarehouseDetailsDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                IsMainWarehouse = warehouse.IsMainWarehouse,
                StockItems = stockItems
            };

            return View(viewModel);
        }

       
        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            // Prepare Dropdowns
            await PopulateTransferDropdowns();
            return View(new StockTransferDto());
        }

        
        [HttpPost]
        public async Task<IActionResult> Transfer(StockTransferDto dto)
        {
            if (dto.FromWarehouseId == dto.ToWarehouseId)
            {
                ModelState.AddModelError("", "Source and Destination cannot be the same.");
            }

            if (ModelState.IsValid)
            {
                bool success = await _warehouseService.TransferStock(dto);
                if (success)
                {
                    TempData["SuccessMessage"] = "Stock transferred successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Transfer failed. Check stock levels.");
            }

            await PopulateTransferDropdowns();
            return View(dto);
        }

       
        [HttpGet]
        public async Task<IActionResult> Logs()
        {
            var logs = await _warehouseService.GetTransferLogs();
            return View(logs);
        }

        
        private async Task PopulateTransferDropdowns()
        {
            var warehouses = await _warehouseService.GetAllWarehouses();
            
            var products = await _productService.GetAllProducts();

            ViewBag.WarehouseList = warehouses.Select(w => new SelectListItem
            {
                Value = w.Id.ToString(),
                Text = w.Name
            });

            ViewBag.ProductList = products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetStockByWarehouse(int warehouseId)
        {
            var stockItems = await _warehouseService.GetWarehouseStock(warehouseId);

            var formattedItems = stockItems.Select(s => new
            {
                id = s.ProductPackageId, 
                quantity = s.Quantity,  

                displayText = $"{s.ProductName}" +
                              (!string.IsNullOrEmpty(s.VariationName) ? $" - {s.VariationName}" : "") +
                              $" - {s.PackageName}"
            });

            return Json(formattedItems);
        }
    }
}
