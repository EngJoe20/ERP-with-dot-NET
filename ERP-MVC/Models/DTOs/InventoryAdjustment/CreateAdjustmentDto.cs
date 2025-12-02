using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.InventoryAdjustment
{
    public class CreateAdjustmentDto
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductPackageId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal NewQuantity { get; set; } // "Type the new quantity"

        [Required]
        public string Reason { get; set; } // "Stolen", "Expired"

        // public int? UserId { get; set; } // We will handle this later
    }
}
