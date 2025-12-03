using ERP_API.Application.DTOs.InventoryAdjustment;
using ERP_API.DataAccess.Entities.InventoryAdjustment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces
{
    public interface IInventoryAdjustmentService
    {
        Task<InventoryAdjustment> CreateAdjustmentAsync(CreateAdjustmentDto dto);
        Task<IEnumerable<AdjustmentLogDto>> GetAdjustmentLogsAsync();
    }
}
