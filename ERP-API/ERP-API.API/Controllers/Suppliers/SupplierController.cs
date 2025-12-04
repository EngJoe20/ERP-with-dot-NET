using ERP_API.Application.DTOs.Suppliers;
using ERP_API.Application.Interfaces.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ERP_API.API.Controllers.Suppliers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _supplierService.GetAllSuppliersAsync();
            return Ok(new { success = true, data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _supplierService.GetSupplierAsync(id);
            if (supplier == null)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, data = supplier });
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _supplierService.GetSupplierDetailsAsync(id);
            if (details == null)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, data = details });
        }

        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetTransactions(int id)
        {
            var tx = await _supplierService.GetSupplierTransactionsAsync(id);
            return Ok(new { success = true, data = tx });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var supplier = await _supplierService.CreateSupplierAsync(dto);
            return Ok(new { success = true, data = supplier });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            var result = await _supplierService.UpdateSupplierAsync(id, dto);
            if (result == null)
                return NotFound(new { success = false, message = "Supplier not found" });

            return Ok(new { success = true, data = result });
        }

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
