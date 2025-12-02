using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Warehouse
{
    public class StockTransferLogDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        // Readable Names instead of IDs
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }

        // Product Details combined
        public string ProductName { get; set; }
        public string VariationName { get; set; }
        public string PackageType { get; set; }

        public decimal Quantity { get; set; }
    }
}
