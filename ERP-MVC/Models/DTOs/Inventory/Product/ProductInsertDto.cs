using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Inventory.Product
{
    public class ProductInsertDto
    {
        // Product Info
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }

        // Variation Info
        public string VariationName { get; set; }
        public string? Flavor { get; set; }
        public string? Size { get; set; }

        // Package Info
        public int PackageTypeId { get; set; }
        public decimal QinP { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }


        public decimal InitialQuantity { get; set; }
    }
}
