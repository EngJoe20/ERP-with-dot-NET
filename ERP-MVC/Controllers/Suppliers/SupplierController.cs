using ERP_MVC.Models.DTOs.Suppliers;
using ERP_MVC.Services.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ERP_MVC.Controllers.Suppliers
{
    public class SupplierController : Controller
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: Supplier List
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return View(suppliers);
        }

        // GET: Add Supplier Form
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateSupplierDto());
        }

        // POST: Create Supplier
        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool success = await _supplierService.CreateSupplierAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Supplier added successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error saving supplier. Please try again.");
            return View(dto);
        }

        // GET: Supplier Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetSupplierDetailsAsync(id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }



        // GET: Edit Supplier Form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
                return NotFound();

            var updateDto = new UpdateSupplierDto
            {
                SupplierName = supplier.SupplierName,
                TaxNumber = supplier.TaxNumber,
                Email = supplier.Email,
                Phone = supplier.Phone,
                OpeningBalance = supplier.OpeningBalance,
                TotalBalance = supplier.TotalBalance,
                Description = supplier.Description
            };

            ViewBag.SupplierId = id;
            return View(updateDto);
        }

        // POST: Update Supplier
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool success = await _supplierService.UpdateSupplierAsync(dto.Id, dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Supplier updated successfully!";
                return RedirectToAction("Details", new { id = dto.Id });
            }

            ModelState.AddModelError("", "Error updating supplier. Please try again.");
            return View(dto);
        }

        // POST: Delete Supplier
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _supplierService.DeleteSupplierAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Supplier deleted successfully!";
            else
                TempData["ErrorMessage"] = "Error deleting supplier.";

            return RedirectToAction("Index");
        }

        // Optional: AJAX endpoint to fetch transactions
        [HttpGet]
        public async Task<IActionResult> Transactions(int supplierId)
        {
            var transactions = await _supplierService.GetSupplierTransactionsAsync(supplierId);
            return Json(transactions);
        }
    }
}
