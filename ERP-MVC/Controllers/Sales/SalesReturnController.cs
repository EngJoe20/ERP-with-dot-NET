using ERP_MVC.Models.DTOs.Sales;
using ERP_MVC.Services.Sales;
using ERP_MVC.Services.Customers;
using ERP_MVC.Services.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_MVC.Controllers.Sales
{
    [Authorize]
    public class SalesReturnController : Controller
    {
        private readonly SalesReturnService _returnService;
        private readonly CustomerService _customerService;
        private readonly WarehouseService _warehouseService;

        public SalesReturnController(
            SalesReturnService returnService,
            CustomerService customerService,
            WarehouseService warehouseService)
        {
            _returnService = returnService;
            _customerService = customerService;
            _warehouseService = warehouseService;
        }

        // GET: Sales Returns List
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var returns = await _returnService.GetAllReturnsAsync();
            return View("~/Views/Sales/return/Index.cshtml", returns);
        }

        // GET: Return Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var returnEntity = await _returnService.GetReturnByIdAsync(id);

            if (returnEntity == null)
                return NotFound();

            return View(returnEntity);
        }

        // GET: Create Return Form (without invoice)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View("~/Views/Sales/return/Create.cshtml", new CreateSalesReturnDto());
        }

        // POST: Create Return
        [HttpPost]
        public async Task<IActionResult> Create(CreateSalesReturnDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View("~/Views/Sales/return/Create.cshtml", dto);
            }

            bool success = await _returnService.CreateReturnAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Sales return created successfully!";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns();
            ModelState.AddModelError("", "Error creating sales return. Please try again.");
            return View("~/Views/Sales/return/Create.cshtml", dto);
        }

        // POST: Delete Return
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _returnService.DeleteReturnAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Sales return deleted successfully!";
            else
                TempData["ErrorMessage"] = "Error deleting sales return.";

            return RedirectToAction("Index");
        }

        // AJAX: Get Products (no stock check needed for returns)
        [HttpGet]
        public async Task<JsonResult> GetProductsByWarehouse(int warehouseId)
        {
            var stockItems = await _warehouseService.GetWarehouseStock(warehouseId);

            var formattedItems = stockItems.Select(s => new
            {
                id = s.ProductPackageId,
                name = $"{s.ProductName}" +
                       (!string.IsNullOrEmpty(s.VariationName) ? $" - {s.VariationName}" : "") +
                       $" ({s.PackageName})"
            });

            return Json(formattedItems);
        }

        private async Task PopulateDropdowns()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var warehouses = await _warehouseService.GetAllWarehouses();

            ViewBag.CustomerList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CustomerName
            });

            ViewBag.WarehouseList = warehouses.Select(w => new SelectListItem
            {
                Value = w.Id.ToString(),
                Text = w.Name
            });
        }
    }
}