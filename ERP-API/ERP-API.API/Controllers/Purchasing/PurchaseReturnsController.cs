using ERP_API.Application.DTOs.Purchasing.PurchaseReturn;
using ERP_API.Application.Interfaces.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Purchasing
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReturnsController : ControllerBase
    {
        private readonly IPurchaseReturnService _service;

        public PurchaseReturnsController(IPurchaseReturnService service)
        {
            _service = service;
        }

        
        //get all purchase returns
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

        //get purchase return by ID
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

        //get purchase returns by supplier
        [HttpGet("supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(Guid supplierId)
        {
            try
            {
                var returns = await _service.GetReturnsBySupplierAsync(supplierId);
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //create a new purchase return
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseReturnDto dto)
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


        //ddelete purchase return
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