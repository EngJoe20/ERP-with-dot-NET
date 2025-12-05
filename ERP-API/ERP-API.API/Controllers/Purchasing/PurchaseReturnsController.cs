using ERP_API.Application.DTOs.Purchasing;
using ERP_API.Application.DTOs.Purchasing.PurchaseReturn;
using ERP_API.Application.Interfaces.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers.Purchasing
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseReturnsController : ControllerBase
    {
        private readonly IPurchaseReturnService _purchaseReturnService;

        public PurchaseReturnsController(IPurchaseReturnService purchaseReturnService)
        {
            _purchaseReturnService = purchaseReturnService;
        }


        /// Get all purchase returns
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var returns = await _purchaseReturnService.GetAllReturnsAsync();
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

   
        /// Get purchase return by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var returnEntity = await _purchaseReturnService.GetReturnByIdAsync(id);

                if (returnEntity == null)
                    return NotFound(new { message = $"Purchase return with ID {id} not found" });

                return Ok(returnEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Get purchase returns by supplier
        [HttpGet("Supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            try
            {
                var returns = await _purchaseReturnService.GetReturnsBySupplierAsync(supplierId);
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Create new purchase return (without invoice)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseReturnDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get user ID from JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var returnEntity = await _purchaseReturnService.CreateReturnAsync(dto);

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


        /// Delete purchase return
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _purchaseReturnService.DeleteReturnAsync(id);

                if (!success)
                    return NotFound(new { message = $"Purchase return with ID {id} not found" });

                return Ok(new { message = "Purchase return deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}