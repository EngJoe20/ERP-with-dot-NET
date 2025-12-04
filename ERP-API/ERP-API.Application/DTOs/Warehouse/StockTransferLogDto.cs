using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Warehouse
{
    public class StockTransferLogDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        // Readable Names instead of IDs
        public required string FromWarehouse { get; set; }
        public required string ToWarehouse { get; set; }

        // Product Details combined
        public required string ProductName { get; set; }
        public required string VariationName { get; set; }
        public required string PackageType { get; set; }

        public decimal Quantity { get; set; }
    }
}
