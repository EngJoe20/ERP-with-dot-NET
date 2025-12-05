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
    public class SalesInvoiceController : Controller
    {
        private readonly SalesInvoiceService _invoiceService;
        private readonly CustomerService _customerService;
        private readonly WarehouseService _warehouseService;

        public SalesInvoiceController(
            SalesInvoiceService invoiceService,
            CustomerService customerService,
            WarehouseService warehouseService)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _warehouseService = warehouseService;
        }

        // GET: Sales Invoices List
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View(invoices);
        }

        // GET: Invoice Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // GET: Create Invoice Form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new CreateSalesInvoiceDto());
        }

        // POST: Create Invoice
        [HttpPost]
        public async Task<IActionResult> Create(CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(dto);
            }

            bool success = await _invoiceService.CreateInvoiceAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Sales invoice created successfully!";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns();
            ModelState.AddModelError("", "Error creating sales invoice. Please try again.");
            return View(dto);
        }

        // POST: Delete Invoice
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _invoiceService.DeleteInvoiceAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Sales invoice deleted successfully!";
            else
                TempData["ErrorMessage"] = "Error deleting sales invoice.";

            return RedirectToAction("Index");
        }

        // AJAX: Get Products by Warehouse with Stock
        [HttpGet]
        public async Task<JsonResult> GetProductsByWarehouse(int warehouseId)
        {
            var stockItems = await _warehouseService.GetWarehouseStock(warehouseId);

            var formattedItems = stockItems.Select(s => new
            {
                id = s.ProductPackageId,
                name = $"{s.ProductName}" +
                       (!string.IsNullOrEmpty(s.VariationName) ? $" - {s.VariationName}" : "") +
                       $" ({s.PackageName})",
                availableStock = s.Quantity
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