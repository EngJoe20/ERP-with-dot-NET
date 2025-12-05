using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces;
using ERP_API.Application.Interfaces.Customers;
using ERP_API.Application.Interfaces.Finance;
using ERP_API.Application.Interfaces.Suppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_API.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ReceiptOrderController : ControllerBase
    {
        private readonly IReceiptOrderService _receiptOrderService;
        private readonly ICustomerService _customerService;
        private readonly ISupplierService _supplierService;

        public ReceiptOrderController(
            IReceiptOrderService receiptOrderService,
            ICustomerService customerService,
            ISupplierService supplierService)
        {
            _receiptOrderService = receiptOrderService;
            _customerService = customerService;
            _supplierService = supplierService;
        }

        /// <summary>
        /// Get all receipt orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var receipts = await _receiptOrderService.GetAllReceiptOrdersAsync();
                return Ok(new { success = true, data = receipts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get customers and suppliers for receipt order creation
        /// </summary>
        [HttpGet("create-data")]
        public async Task<IActionResult> GetCreateData()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                return Ok(new { success = true, data = new { customers, suppliers } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new receipt order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceiptOrderDto createDto)
        {
            try
            {
                // Validate reference table
                var validTables = new[] { "customertransactions", "suppliertransactions", "profitsources", "expenses" };
                if (!validTables.Contains(createDto.ReferenceTable.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Invaild reference" });
                }

                // Validate customer/supplier ID based on transaction type
                if (createDto.ReferenceTable.ToLower() == "customertransactions" && !createDto.CustomerId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Please choose customer" });
                }

                if (createDto.ReferenceTable.ToLower() == "suppliertransactions" && !createDto.SupplierId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Please choose Suppler" });
                }

                var userId = GetCurrentUserId();
                var id = await _receiptOrderService.CreateReceiptOrderAsync(createDto, userId);

                return Ok(new { success = true, message = "Receipt order created successfully", id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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
