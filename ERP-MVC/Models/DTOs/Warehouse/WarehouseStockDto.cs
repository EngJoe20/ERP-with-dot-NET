using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Warehouse
{
    public class WarehouseStockDto
    {
        public int StockId { get; set; }
        public int ProductPackageId { get; set; }
        public string ProductName { get; set; }     // e.g. "Tiger Chips"
        public string VariationName { get; set; }   // e.g. "Chili"
        public string PackageName { get; set; }     // e.g. "Carton"
        public decimal Quantity { get; set; }       // e.g. 20
    }
}
