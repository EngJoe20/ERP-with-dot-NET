using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Inventory.Product
{
    // Feature 2: Adding a new Package to existing Variation (e.g., adding "Carton" to "Cheese Flavor")
    public class PackageLinkInsertDto
    {
        public int PackageTypeId { get; set; }
        public decimal QinP { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }

        public decimal InitialQuantity { get; set; }
    }
}
