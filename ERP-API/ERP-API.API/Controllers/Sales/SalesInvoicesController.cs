using ERP_API.Application.DTOs.Sales.SalesInvoice;
using ERP_API.Application.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Sales
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesInvoicesController : ControllerBase
    {
        private readonly ISalesInvoiceService _service;

        public SalesInvoicesController(ISalesInvoiceService service)
        {
            _service = service;
        }


        //get all sales invoices
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


        //get sales invoice by ID
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


        //get sales invoices by customer
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(Guid customerId)
        {
            try
            {
                var invoices = await _service.GetInvoicesByCustomerAsync(customerId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //create a new sales invoice
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesInvoiceDto dto)
        {
            try
            {
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


        //delete sales invoice
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