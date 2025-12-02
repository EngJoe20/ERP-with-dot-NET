using ERP_API.Application.Interfaces;
using ERP_API.Application.DTOs.Warehouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ERP_API.API.Controllers
{

    [Authorize(Roles = "users")]
    [Route("api/[controller]")]
    [ApiController]

    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseInsertDto dto)
        {
            var createdWarehouse = await _warehouseService.AddWarehouseAsync(dto);
            return Ok(createdWarehouse);
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> TransferStock(StockTransferDto dto)
        {
            if (dto.FromWarehouseId == dto.ToWarehouseId)
            {
                return BadRequest("Source and Destination warehouses cannot be the same.");
            }

            bool success = await _warehouseService.TransferStockAsync(dto);

            if (!success)
            {
                return BadRequest("Transfer Failed: Insufficient stock or invalid item.");
            }

            return Ok("Transfer Successful");
        }

        [HttpGet("{id}/Stock")]
        public async Task<IActionResult> GetStock(int id)
        {
            var result = await _warehouseService.GetWarehouseStockAsync(id);
            return Ok(result);
        }

        [HttpGet("Logs")]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _warehouseService.GetTransferLogsAsync();
            return Ok(logs);
        }


    }
}
