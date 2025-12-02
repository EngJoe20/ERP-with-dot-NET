using ERP_API.DataAccess.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Inventory
{
    public class ProductPackage 
    {
        public int Id { get; set; }

        // Relationship: Belongs to ONE Variation
        public int ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; }

        // Relationship: Uses ONE Package Type definition
        public int PackageTypeId { get; set; }
        public PackageType PackageType { get; set; }

        // Quantity in Package (e.g., 50 if it's a 50KG bag)
        public decimal QinP { get; set; }

        // COST PRICE: As per your rule, this is the price for "1 unit" (e.g. 1 KG), not the whole package
        public decimal PurchasePrice { get; set; }

        public decimal SalesPrice { get; set; }

        public string Barcode { get; set; } // Generated Automatically

        public ICollection<WarehouseStock> WarehouseStocks { get; set; }
    }
}
