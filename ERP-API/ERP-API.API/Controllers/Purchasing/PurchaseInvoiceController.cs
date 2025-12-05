using ERP_API.Application.DTOs.Purchasing;
using ERP_API.Application.DTOs.Purchasing.PurchaseInvoice;
using ERP_API.Application.Interfaces.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Purchasing
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseInvoicesController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        public PurchaseInvoicesController(IPurchaseInvoiceService purchaseInvoiceService)
        {
            _purchaseInvoiceService = purchaseInvoiceService;
        }

        /// Get all purchase invoices
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var invoices = await _purchaseInvoiceService.GetAllInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Get purchase invoice by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var invoice = await _purchaseInvoiceService.GetInvoiceByIdAsync(id);

                if (invoice == null)
                    return NotFound(new { message = $"Invoice with ID {id} not found" });

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

  
        /// Get purchase invoices by supplier
        [HttpGet("Supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            try
            {
                var invoices = await _purchaseInvoiceService.GetInvoicesBySupplierAsync(supplierId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// Create new purchase invoice
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseInvoiceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get user ID from JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var invoice = await _purchaseInvoiceService.CreateInvoiceAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = invoice.Id },
                    invoice
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Delete purchase invoice
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _purchaseInvoiceService.DeleteInvoiceAsync(id);

                if (!success)
                    return NotFound(new { message = $"Invoice with ID {id} not found" });

                return Ok(new { message = "Invoice deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}