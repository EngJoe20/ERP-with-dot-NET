using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Product
{
    public class VariationInsertDto
    {
        // Variation Details
        public required string VariationName { get; set; } // e.g., "Cheese"
        public string? Flavor { get; set; }
        public string? Size { get; set; }

        // It must have at least one package to be sold
        public int PackageTypeId { get; set; }
        public decimal QinP { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }

        public decimal InitialQuantity { get; set; }
    }
}
