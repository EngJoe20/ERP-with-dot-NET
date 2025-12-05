using ERP_API.Application.DTOs.Suppliers;
using ERP_API.Application.Interfaces.Suppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_API.API.Controllers.Suppliers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // ---------------- GET ALL ----------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(new { success = true, data = suppliers });
        }

        // ---------------- GET BY ID ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _supplierService.GetSupplierAsync(id);
            if (supplier == null)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, data = supplier });
        }

        // ---------------- GET WITH DETAILS ----------------
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetSupplierDetails(int id)
        {
            var details = await _supplierService.GetSupplierDetailsAsync(id);
            if (details == null)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, data = details });
        }

        // ---------------- GET TRANSACTIONS ONLY ----------------
        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetTransactions(int id)
        {
            var transactions = await _supplierService.GetSupplierTransactionsAsync(id);
            return Ok(new { success = true, data = transactions });
        }

        // ---------------- CREATE ----------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var supplier = await _supplierService.CreateSupplierAsync(dto);
            return Ok(new { success = true, data = supplier });
        }

        // ---------------- UPDATE ----------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            var supplier = await _supplierService.UpdateSupplierAsync(id, dto);
            if (supplier == null)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, data = supplier });
        }

        // ---------------- DELETE ----------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _supplierService.DeleteSupplierAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, message = "Supplier deleted successfully" });
        }
    }
}
