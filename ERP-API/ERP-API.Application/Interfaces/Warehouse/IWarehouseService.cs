using ERP_API.Application.DTOs.Warehouse;
using ERP_API.DataAccess.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<Warehouse> AddWarehouseAsync(WarehouseInsertDto dto);

        Task<IEnumerable<WarehouseItemDto>> GetAllWarehousesAsync();

        Task<bool> TransferStockAsync(StockTransferDto dto);

        Task<IEnumerable<WarehouseStockDto>> GetWarehouseStockAsync(int warehouseId);

        Task<IEnumerable<StockTransferLogDto>> GetTransferLogsAsync();

    }
}
