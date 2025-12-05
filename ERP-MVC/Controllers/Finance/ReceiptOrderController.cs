using ERP_MVC.Models.DTOs.Finance;
using ERP_MVC.Models.ViewModels.Finance;
using ERP_MVC.Services.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_MVC.Controllers
{
    [Authorize]
    public class ReceiptOrderController : Controller
    {
        private readonly ReceiptOrderService _receiptOrderService;

        public ReceiptOrderController(ReceiptOrderService receiptOrderService)
        {
            _receiptOrderService = receiptOrderService;
        }

        // GET: ReceiptOrder/Index - List all receipts
        public async Task<IActionResult> Index()
        {
            try
            {
                var receipts = await _receiptOrderService.GetAllReceiptOrdersAsync();
                return View(receipts);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading receipts: {ex.Message}";
                return View(new List<ReceiptOrderDto>());
            }
        }


        // GET: ReceiptOrder/Create - Show create form
        public async Task<IActionResult> Create()
        {
            try
            {
                var createData = await _receiptOrderService.GetCreateDataAsync();

                var viewModel = new CreateReceiptOrderViewModel
                {
                    Customers = createData?.Customers ?? new List<CustomerDto>(),
                    Suppliers = createData?.Suppliers ?? new List<SupplierDto>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading form data: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReceiptOrder/Create - Submit new receipt
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceiptOrderDto createDto)
        {
            try
            {
                // Validate reference table
                var validTables = new[] { "customertransactions", "suppliertransactions", "profitsources", "expenses" };
                if (!validTables.Contains(createDto.ReferenceTable.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Invalid reference" });
                }

                // Validate customer/supplier ID based on transaction type
                if (createDto.ReferenceTable.ToLower() == "customertransactions" && !createDto.CustomerId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Please choose customer" });
                }

                if (createDto.ReferenceTable.ToLower() == "suppliertransactions" && !createDto.SupplierId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Please choose Supplier" });
                }

                // Get userId from authenticated user (MVC has [Authorize])
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                // Call service with userId from authentication, not from DTO
                var id = await _receiptOrderService.CreateReceiptOrderAsync(createDto, userId);

                return Ok(new { success = true, message = "Receipt order created successfully", id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: ReceiptOrder/Edit/5 - Show edit form
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var receipt = await _receiptOrderService.GetReceiptOrderByIdAsync(id);
                if (receipt == null)
                {
                    TempData["ErrorMessage"] = "Receipt order not found";
                    return RedirectToAction(nameof(Index));
                }

                var createData = await _receiptOrderService.GetCreateDataAsync();

                var viewModel = new UpdateReceiptOrderViewModel
                {
                    Id = receipt.Id,
                    Amount = receipt.CreditAmount,
                    Description = receipt.EntryDescription,
                    Customers = createData?.Customers ?? new List<CustomerDto>(),
                    Suppliers = createData?.Suppliers ?? new List<SupplierDto>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading receipt: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReceiptOrder/Edit/5 - Update receipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateReceiptOrderViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Invalid receipt ID";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var createData = await _receiptOrderService.GetCreateDataAsync();
                model.Customers = createData?.Customers ?? new List<CustomerDto>();
                model.Suppliers = createData?.Suppliers ?? new List<SupplierDto>();
                return View(model);
            }

            try
            {
                var dto = new UpdateReceiptOrderDto
                {
                    ReferenceTable = model.ReferenceTable,
                    CustomerId = model.CustomerId,
                    SupplierId = model.SupplierId,
                    Amount = model.Amount,
                    Description = model.Description,
                    ExpenseName = model.ExpenseName,
                    SourceName = model.SourceName
                };

                var success = await _receiptOrderService.UpdateReceiptOrderAsync(id, dto);

                if (success)
                {
                    TempData["SuccessMessage"] = "Receipt order updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update receipt order";
                    var createData = await _receiptOrderService.GetCreateDataAsync();
                    model.Customers = createData?.Customers ?? new List<CustomerDto>();
                    model.Suppliers = createData?.Suppliers ?? new List<SupplierDto>();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating receipt: {ex.Message}";
                var createData = await _receiptOrderService.GetCreateDataAsync();
                model.Customers = createData?.Customers ?? new List<CustomerDto>();
                model.Suppliers = createData?.Suppliers ?? new List<SupplierDto>();
                return View(model);
            }
        }

        // POST: ReceiptOrder/Delete/5 - Delete receipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _receiptOrderService.DeleteReceiptOrderAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Receipt order deleted successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete receipt order";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting receipt: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}