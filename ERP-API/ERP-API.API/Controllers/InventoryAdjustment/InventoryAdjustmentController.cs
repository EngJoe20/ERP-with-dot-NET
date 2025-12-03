using ERP_API.Application.Interfaces;
using ERP_API.Application.DTOs.InventoryAdjustment;
using ERP_API.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP_API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryAdjustmentController : ControllerBase
    {
        private readonly IInventoryAdjustmentService _inventoryAdjustmentService;

        public InventoryAdjustmentController(IInventoryAdjustmentService inventoryAdjustmentService)
        {
            _inventoryAdjustmentService = inventoryAdjustmentService;
        }

        // POST: api/InventoryAdjustment/Adjustments
        [HttpPost("Adjustments")]
        public async Task<IActionResult> CreateAdjustment(CreateAdjustmentDto dto)
        {
            var result = await _inventoryAdjustmentService.CreateAdjustmentAsync(dto);

            if (result == null)
            {
                return BadRequest("No changes made.");
            }

            return Ok(new { Message = "Adjustment recorded successfully.", AdjustmentId = result.Id });
        }


        [HttpGet("Adjustments")]
        public async Task<IActionResult> GetAdjustmentLogs()
        {
            var logs = await _inventoryAdjustmentService.GetAdjustmentLogsAsync();
            return Ok(logs);
        }
    }
}
