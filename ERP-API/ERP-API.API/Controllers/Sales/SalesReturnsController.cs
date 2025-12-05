using ERP_API.Application.DTOs.Sales;
using ERP_API.Application.DTOs.Sales.SalesReturn;
using ERP_API.Application.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesReturnsController : ControllerBase
    {
        private readonly ISalesReturnService _salesReturnService;

        public SalesReturnsController(ISalesReturnService salesReturnService)
        {
            _salesReturnService = salesReturnService;
        }


        /// Get all sales returns
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var returns = await _salesReturnService.GetAllReturnsAsync();
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Get sales return by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var returnEntity = await _salesReturnService.GetReturnByIdAsync(id);

                if (returnEntity == null)
                    return NotFound(new { message = $"Sales return with ID {id} not found" });

                return Ok(returnEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Get sales returns by customer
        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            try
            {
                var returns = await _salesReturnService.GetReturnsByCustomerAsync(customerId);
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Create new sales return (without invoice)
        [HttpPost]
        [Authorize(Roles = "SystemManager,Accountant")]
        public async Task<IActionResult> Create([FromBody] CreateSalesReturnDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get user ID from JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var returnEntity = await _salesReturnService.CreateReturnAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = returnEntity.Id },
                    returnEntity
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        /// Delete sales return
        [HttpDelete("{id}")]
        [Authorize(Roles = "SystemManager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _salesReturnService.DeleteReturnAsync(id);

                if (!success)
                    return NotFound(new { message = $"Sales return with ID {id} not found" });

                return Ok(new { message = "Sales return deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}