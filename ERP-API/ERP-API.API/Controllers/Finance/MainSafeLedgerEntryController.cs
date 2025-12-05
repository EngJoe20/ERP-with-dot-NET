using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERP_API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class MainSafeLedgerEntryController : ControllerBase
    {
        private readonly IMainSafeLedgerEntryService _ledgerEntryService;

        public MainSafeLedgerEntryController(IMainSafeLedgerEntryService ledgerEntryService)
        {
            _ledgerEntryService = ledgerEntryService;
        }

        /// <summary>
        /// Get all ledger entries
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entries = await _ledgerEntryService.GetAllLedgerEntriesAsync();
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger entry by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var entry = await _ledgerEntryService.GetLedgerEntryByIdAsync(id);
                if (entry == null)
                    return NotFound(new { success = false, message = "Ledger entry not found" });

                return Ok(new { success = true, data = entry });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger entry details by ID
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var details = await _ledgerEntryService.GetLedgerEntryDetailsAsync(id);
                if (details == null)
                    return NotFound(new { success = false, message = "Ledger entry not found" });

                return Ok(new { success = true, data = details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger entries by Main Safe ID
        /// </summary>
        [HttpGet("main-safe/{mainSafeId}")]
        public async Task<IActionResult> GetByMainSafeId(int mainSafeId)
        {
            try
            {
                var entries = await _ledgerEntryService.GetLedgerEntriesByMainSafeIdAsync(mainSafeId);
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get filtered ledger entries
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetFiltered([FromBody] MainSafeLedgerEntryFilterDto filter)
        {
            try
            {
                var entries = await _ledgerEntryService.GetFilteredLedgerEntriesAsync(filter);
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger entries by date range
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var entries = await _ledgerEntryService.GetLedgerEntriesByDateRangeAsync(startDate, endDate);
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger entries by direction (In/Out)
        /// </summary>
        [HttpGet("direction/{direction}")]
        public async Task<IActionResult> GetByDirection(string direction)
        {
            try
            {
                var entries = await _ledgerEntryService.GetLedgerEntriesByDirectionAsync(direction);
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger entries by reference table
        /// </summary>
        [HttpGet("reference/{referenceTable}")]
        public async Task<IActionResult> GetByReferenceTable(string referenceTable)
        {
            try
            {
                var entries = await _ledgerEntryService.GetLedgerEntriesByReferenceTableAsync(referenceTable);
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ledger summary
        /// </summary>
        [HttpGet("summary/{mainSafeId}")]
        public async Task<IActionResult> GetSummary(int mainSafeId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var summary = await _ledgerEntryService.GetLedgerSummaryAsync(mainSafeId, startDate, endDate);
                return Ok(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get latest entries
        /// </summary>
        [HttpGet("latest/{mainSafeId}")]
        public async Task<IActionResult> GetLatest(int mainSafeId, [FromQuery] int count = 10)
        {
            try
            {
                var entries = await _ledgerEntryService.GetLatestEntriesAsync(mainSafeId, count);
                return Ok(new { success = true, data = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new ledger entry
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMainSafeLedgerEntryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                var userId = GetCurrentUserId();
                var id = await _ledgerEntryService.CreateLedgerEntryAsync(createDto, userId);

                return CreatedAtAction(nameof(GetById), new { id }, new { success = true, message = "Ledger entry created successfully", id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing ledger entry
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMainSafeLedgerEntryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                var exists = await _ledgerEntryService.LedgerEntryExistsAsync(id);
                if (!exists)
                    return NotFound(new { success = false, message = "Ledger entry not found" });

                await _ledgerEntryService.UpdateLedgerEntryAsync(id, updateDto);
                return Ok(new { success = true, message = "Ledger entry updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a ledger entry
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exists = await _ledgerEntryService.LedgerEntryExistsAsync(id);
                if (!exists)
                    return NotFound(new { success = false, message = "Ledger entry not found" });

                await _ledgerEntryService.DeleteLedgerEntryAsync(id);
                return Ok(new { success = true, message = "Ledger entry deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check if ledger entry exists
        /// </summary>
        [HttpHead("{id}")]
        [HttpGet("{id}/exists")]
        public async Task<IActionResult> Exists(int id)
        {
            try
            {
                var exists = await _ledgerEntryService.LedgerEntryExistsAsync(id);
                if (exists)
                    return Ok(new { success = true, exists = true });
                else
                    return NotFound(new { success = false, exists = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim ?? "0";
        }
    }
}