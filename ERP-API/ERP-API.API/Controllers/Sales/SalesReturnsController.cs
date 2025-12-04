using ERP_API.Application.DTOs.Sales.SalesReturn;
using ERP_API.Application.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Sales
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesReturnsController : ControllerBase
    {
        private readonly ISalesReturnService _service;

        public SalesReturnsController(ISalesReturnService service)
        {
            _service = service;
        }

        //get all sales returns
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var returns = await _service.GetAllReturnsAsync();
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //get sales return by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var returnEntity = await _service.GetReturnByIdAsync(id);

                if (returnEntity == null)
                    return NotFound(new { message = $"Return with ID {id} not found" });

                return Ok(returnEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //get sales returns by customer
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(Guid customerId)
        {
            try
            {
                var returns = await _service.GetReturnsByCustomerAsync(customerId);
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //create a new sales return
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesReturnDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "User not authenticated" });

                var userId = Guid.Parse(userIdClaim);

                var returnEntity = await _service.CreateReturnAsync(dto, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = returnEntity.Id },
                    returnEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //delete sales return
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteReturnAsync(id);

                if (!result)
                    return NotFound(new { message = $"Return with ID {id} not found" });

                return Ok(new { message = "Return deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}