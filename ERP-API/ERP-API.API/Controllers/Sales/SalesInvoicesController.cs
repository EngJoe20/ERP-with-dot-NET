using ERP_API.Application.DTOs.Sales;
using ERP_API.Application.DTOs.Sales.SalesInvoice;
using ERP_API.Application.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesInvoicesController : ControllerBase
    {
        private readonly ISalesInvoiceService _salesInvoiceService;

        public SalesInvoicesController(ISalesInvoiceService salesInvoiceService)
        {
            _salesInvoiceService = salesInvoiceService;
        }


        /// Get all sales invoices
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var invoices = await _salesInvoiceService.GetAllInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Get sales invoice by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var invoice = await _salesInvoiceService.GetInvoiceByIdAsync(id);

                if (invoice == null)
                    return NotFound(new { message = $"Invoice with ID {id} not found" });

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Get sales invoices by customer
        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            try
            {
                var invoices = await _salesInvoiceService.GetInvoicesByCustomerAsync(customerId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Create new sales invoice
        [HttpPost]
        [Authorize(Roles = "SystemManager,Accountant")]
        public async Task<IActionResult> Create([FromBody] CreateSalesInvoiceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get user ID from JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var invoice = await _salesInvoiceService.CreateInvoiceAsync(dto);

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


        /// Delete sales invoice
        [HttpDelete("{id}")]
        [Authorize(Roles = "SystemManager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _salesInvoiceService.DeleteInvoiceAsync(id);

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