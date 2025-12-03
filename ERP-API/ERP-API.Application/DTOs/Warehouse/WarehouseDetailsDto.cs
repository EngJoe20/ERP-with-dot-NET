using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Warehouse
{
    public class WarehouseDetailsDto
    {
        // Header Info
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public bool IsMainWarehouse { get; set; }

        // The Inventory Table
        public IEnumerable<WarehouseStockDto> StockItems { get; set; }
    }
}
