using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ERP_MVC.Services.Inventory;
using ERP_MVC.Models.DTOs.Inventory;
using ERP_MVC.Services.Warehouse;
using ERP_MVC.Services.Inventory.Product;
using ERP_MVC.Models.DTOs.InventoryAdjustment;
using ERP_MVC.Services.InventoryAdjustment;

namespace ERP_MVC.Controllers.Inventory
{
    public class InventoryAdjustmentController : Controller
    {
        private readonly InventoryAdjustmentService _adjustmentService;
        private readonly WarehouseService _warehouseService;

        public InventoryAdjustmentController(
            InventoryAdjustmentService adjustmentService,
            WarehouseService warehouseService)
        {
            _adjustmentService = adjustmentService;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var logs = await _adjustmentService.GetAdjustmentLogsAsync();
            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateWarehouseDropdown();

            return View(new CreateAdjustmentDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdjustmentDto dto)
        {
            if (ModelState.IsValid)
            {
                bool success = await _adjustmentService.CreateAdjustmentAsync(dto);

                if (success)
                {
                    TempData["SuccessMessage"] = "Stock level updated successfully.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Failed to update inventory. Please try again.");
            }

            await PopulateWarehouseDropdown();
            return View(dto);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductsByWarehouse(int warehouseId)
        {
            var stockItems = await _warehouseService.GetWarehouseStock(warehouseId);

            var formattedItems = stockItems.Select(s => new
            {
                id = s.ProductPackageId,
                // We assume user wants to set New Quantity, so we must show Current Quantity
                // Format: "Pepsi - Diet (Current: 50)"
                displayText = $"{s.ProductName}" +
                              (!string.IsNullOrEmpty(s.VariationName) && s.VariationName != "string" ? $" - {s.VariationName}" : "") +
                              $" - {s.PackageName}",

                currentQty = s.Quantity // Send this separately so JS can use it if needed
            });

            return Json(formattedItems);
        }

        private async Task PopulateWarehouseDropdown()
        {
            var warehouses = await _warehouseService.GetAllWarehouses();
            ViewBag.WarehouseList = warehouses.Select(w => new SelectListItem
            {
                Value = w.Id.ToString(),
                Text = w.Name
            });
        }
    }
}