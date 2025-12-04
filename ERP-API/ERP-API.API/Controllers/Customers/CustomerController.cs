using ERP_API.Application.DTOs.Customers;
using ERP_API.Application.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_API.API.Controllers.Customers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // ---------------- GET ALL ----------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(new { success = true, data = customers });
        }

        // ---------------- GET BY ID ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
                return NotFound(new { success = false, message = "Customer not found" });

            return Ok(new { success = true, data = customer });
        }

        // ---------------- GET WITH DETAILS ----------------
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetCustomerDetails(int id)
        {
            var details = await _customerService.GetCustomerDetailsAsync(id);
            if (details == null)
                return NotFound(new { success = false, message = "Customer not found" });

            return Ok(new { success = true, data = details });
        }

        // ---------------- GET TRANSACTIONS ONLY ----------------
        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetTransactions(int id)
        {
            var transactions = await _customerService.GetCustomerTransactionsAsync(id);
            return Ok(new { success = true, data = transactions });
        }

        // ---------------- CREATE ----------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            var customer = await _customerService.CreateCustomerAsync(dto);
            return Ok(new { success = true, data = customer });
        }

        // ---------------- UPDATE ----------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
        {
            var customer = await _customerService.UpdateCustomerAsync(id, dto);
            if (customer == null)
                return NotFound(new { success = false, message = "Customer not found" });

            return Ok(new { success = true, data = customer });
        }

        // ---------------- DELETE ----------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Customer not found" });

            return Ok(new { success = true, message = "Customer deleted successfully" });
        }
    }
}
