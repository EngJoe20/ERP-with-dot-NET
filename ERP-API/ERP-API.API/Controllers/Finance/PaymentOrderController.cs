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
    public class PaymentOrderController : ControllerBase
    {
        private readonly IPaymentOrderService _paymentOrderService;
        private readonly ICustomerService _customerService;
        private readonly ISupplierService _supplierService;

        public PaymentOrderController(
            IPaymentOrderService paymentOrderService,
            ICustomerService customerService,
            ISupplierService supplierService)
        {
            _paymentOrderService = paymentOrderService;
            _customerService = customerService;
            _supplierService = supplierService;
        }

        /// <summary>
        /// Get all payment orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var payments = await _paymentOrderService.GetAllPaymentOrdersAsync();
                return Ok(new { success = true, data = payments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get customers and suppliers for payment order creation
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
        /// Create a new payment order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentOrderDto createDto)
        {
            try
            {
                // Validate reference table
                var validTables = new[] { "customertransactions", "suppliertransactions", "profitsources", "expenses" };
                if (!validTables.Contains(createDto.ReferenceTable.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Invailed reference" });
                }

                // Validate customer/supplier ID based on transaction type
                if (createDto.ReferenceTable.ToLower() == "customertransactions" && !createDto.CustomerId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Please choose customer" });
                }

                if (createDto.ReferenceTable.ToLower() == "suppliertransactions" && !createDto.SupplierId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Please choose suppler" });
                }

                var userId = GetCurrentUserId();
                var id = await _paymentOrderService.CreatePaymentOrderAsync(createDto, userId);

                return Ok(new { success = true, message = "Payment order created successfully", id });
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
