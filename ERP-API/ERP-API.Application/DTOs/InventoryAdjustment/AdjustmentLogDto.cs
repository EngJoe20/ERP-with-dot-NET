using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.InventoryAdjustment
{
    public class AdjustmentLogDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public required string WarehouseName { get; set; }
        public required string ProductName { get; set; } // Full name (Tiger - Chili - Carton)

        public required string Type { get; set; } // "Increase" or "Decrease"
        public required string Reason { get; set; }

        public decimal Difference { get; set; } // +100 or -50
    }
}
