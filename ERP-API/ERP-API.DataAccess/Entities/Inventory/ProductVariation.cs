using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Inventory
{
    public class ProductVariation 
    {
        public int Id { get; set; }

        public string Name { get; set; } // e.g., "Maxi Chips Chili"
        public string SKU { get; set; }  // Generated Automatically via code logic later

        public string? Flavor { get; set; }
        public string? Size { get; set; } // Nullable as requested

        // Relationship: Belongs to ONE Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Relationship: Has MANY Packages (Inventory Units)
        public ICollection<ProductPackage> ProductPackages { get; set; }
    }
}
