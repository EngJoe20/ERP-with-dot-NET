using ERP_API.Application.DTOs.Purchasing.PurchaseInvoice;
using ERP_API.Application.Interfaces.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Purchasing
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseInvoicesController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _service;
        public PurchaseInvoicesController(IPurchaseInvoiceService service)
        {
            _service = service;
        }

        
        ///get all purchase invoices
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var invoices = await _service.GetAllInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        //get purchase invoice by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var invoice = await _service.GetInvoiceByIdAsync(id);

                if (invoice == null)
                    return NotFound(new { message = $"Invoice with ID {id} not found" });

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //get purchase invoices by suppliers
        [HttpGet("supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(Guid supplierId)
        {
            try
            {
                var invoices = await _service.GetInvoicesBySupplierAsync(supplierId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //create a new purchase invoice
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseInvoiceDto dto)
        {
            try
            {
                // Get user ID from claims
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "User not authenticated" });

                var userId = Guid.Parse(userIdClaim);

                var invoice = await _service.CreateInvoiceAsync(dto, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = invoice.Id },
                    invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        ///delete purchase invoice
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteInvoiceAsync(id);

                if (!result)
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