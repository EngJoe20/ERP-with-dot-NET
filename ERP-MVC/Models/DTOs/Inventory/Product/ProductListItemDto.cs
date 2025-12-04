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
        public string ProductName { get; set; }
        public string? CategoryName { get; set; } // We will need to fetch this

        // Variation Info
        public string VariationName { get; set; }
        public string SKU { get; set; }

        // Package Info
        public string PackageTypeName { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal QuantityInPackage { get; set; }
    }
}
