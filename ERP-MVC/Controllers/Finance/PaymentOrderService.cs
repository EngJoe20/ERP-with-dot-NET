using ERP_MVC.Models.DTOs.Finance;
using ERP_MVC.Models.ViewModels.Finance;
using ERP_MVC.Services.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_MVC.Controllers
{
    [Authorize]
    public class PaymentOrderController : Controller
    {
        private readonly PaymentOrderService _paymentOrderService;

        public PaymentOrderController(PaymentOrderService paymentOrderService)
        {
            _paymentOrderService = paymentOrderService;
        }

        // GET: PaymentOrder/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var payments = await _paymentOrderService.GetAllPaymentOrdersAsync();
                return View(payments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading payments: {ex.Message}";
                return View(new List<PaymentOrderDto>());
            }
        }

        // GET: PaymentOrder/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var createData = await _paymentOrderService.GetCreateDataAsync();

                var viewModel = new CreatePaymentOrderViewModel
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

        // POST: PaymentOrder/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentOrderDto createDto)
        {
            try
            {
                var validTables = new[] { "customertransactions", "suppliertransactions", "profitsources", "expenses" };
                if (!validTables.Contains(createDto.ReferenceTable.ToLower()))
                    return BadRequest(new { success = false, message = "Invalid reference" });

                if (createDto.ReferenceTable == "customertransactions" && !createDto.CustomerId.HasValue)
                    return BadRequest(new { success = false, message = "Please choose customer" });

                if (createDto.ReferenceTable == "suppliertransactions" && !createDto.SupplierId.HasValue)
                    return BadRequest(new { success = false, message = "Please choose supplier" });

                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { success = false, message = "User not authenticated" });

                var id = await _paymentOrderService.CreatePaymentOrderAsync(createDto, userId);

                return Ok(new { success = true, message = "Payment order created successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: PaymentOrder/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var payment = await _paymentOrderService.GetPaymentOrderByIdAsync(id);
                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Payment order not found";
                    return RedirectToAction(nameof(Index));
                }

                var createData = await _paymentOrderService.GetCreateDataAsync();

                var viewModel = new UpdatePaymentOrderViewModel
                {
                    Id = payment.Id,
                    Amount = payment.DebitAmount,
                    Description = payment.EntryDescription,
                    Customers = createData?.Customers ?? new List<CustomerDto>(),
                    Suppliers = createData?.Suppliers ?? new List<SupplierDto>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading payment: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PaymentOrder/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePaymentOrderViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Invalid payment ID";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var createData = await _paymentOrderService.GetCreateDataAsync();
                model.Customers = createData?.Customers ?? new List<CustomerDto>();
                model.Suppliers = createData?.Suppliers ?? new List<SupplierDto>();
                return View(model);
            }

            try
            {
                var dto = new UpdatePaymentOrderDto
                {
                    ReferenceTable = model.ReferenceTable,
                    CustomerId = model.CustomerId,
                    SupplierId = model.SupplierId,
                    Amount = model.Amount,
                    Description = model.Description,
                    ExpenseName = model.ExpenseName,
                    SourceName = model.SourceName
                };

                var success = await _paymentOrderService.UpdatePaymentOrderAsync(id, dto);

                if (success)
                {
                    TempData["SuccessMessage"] = "Payment order updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update payment order";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating payment: {ex.Message}";
                return View(model);
            }
        }

        // DELETE: PaymentOrder/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _paymentOrderService.DeletePaymentOrderAsync(id);

                if (success)
                    TempData["SuccessMessage"] = "Payment order deleted successfully";
                else
                    TempData["ErrorMessage"] = "Failed to delete payment order";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting payment: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
