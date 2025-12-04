using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Warehouse
{
    public class WarehouseStockDto
    {
        public int StockId { get; set; }
        public int ProductPackageId { get; set; }
        public required string ProductName { get; set; }     // e.g. "Tiger Chips"
        public required string VariationName { get; set; }   // e.g. "Chili"
        public required string PackageName { get; set; }     // e.g. "Carton"
        public decimal Quantity { get; set; }       // e.g. 20
    }
}
