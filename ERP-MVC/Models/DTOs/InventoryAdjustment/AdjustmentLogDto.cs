using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.InventoryAdjustment
{
    public class AdjustmentLogDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string WarehouseName { get; set; }
        public string ProductName { get; set; } // Full name (Tiger - Chili - Carton)

        public string Type { get; set; } // "Increase" or "Decrease"
        public string Reason { get; set; }

        public decimal Difference { get; set; } // +100 or -50
    }
}
