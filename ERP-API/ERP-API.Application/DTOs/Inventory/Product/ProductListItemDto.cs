using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Product
{
    public class ProductListItemDto
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public string? CategoryName { get; set; } // We will need to fetch this

        // Variation Info
        public required string VariationName { get; set; }
        public required string SKU { get; set; }

        // Package Info
        public required string PackageTypeName { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal QuantityInPackage { get; set; }
    }
}
