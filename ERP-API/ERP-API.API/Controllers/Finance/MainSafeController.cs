using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces;
using ERP_API.Application.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MainSafeController : ControllerBase
    {
        private readonly IMainSafeService _mainSafeService;

        public MainSafeController(IMainSafeService mainSafeService)
        {
            _mainSafeService = mainSafeService;
        }

        /// <summary>
        /// Get main safe statement
        /// </summary>
        [HttpGet("statement")]
        public async Task<IActionResult> GetStatement()
        {
            try
            {
                var mainSafe = await _mainSafeService.GetOrCreateDefaultSafeAsync();
                return Ok(new { success = true, data = mainSafe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all main safes
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var safes = await _mainSafeService.GetAllMainSafesAsync();
                return Ok(new { success = true, data = safes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get main safe by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var safe = await _mainSafeService.GetMainSafeAsync(id);
                if (safe == null)
                    return NotFound(new { success = false, message = "Safe not found" });

                return Ok(new { success = true, data = safe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new main safe
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMainSafeDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var safe = await _mainSafeService.CreateMainSafeAsync(createDto, userId);
                return CreatedAtAction(nameof(GetById), new { id = safe.Id }, new { success = true, data = safe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update main safe
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMainSafeDto updateDto)
        {
            try
            {
                var result = await _mainSafeService.UpdateMainSafeAsync(id, updateDto);
                if (!result)
                    return NotFound(new { success = false, message = "Safe not found" });

                return Ok(new { success = true, message = "Safe updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete main safe
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _mainSafeService.DeleteMainSafeAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Safe not found" });

                return Ok(new { success = true, message = "Safe deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}