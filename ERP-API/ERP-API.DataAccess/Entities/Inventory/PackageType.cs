using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Inventory
{
    public class PackageType 
    {
        public int Id { get; set; }

        public string Name { get; set; } // e.g., "Shikara", "Carton"
        public string? Description { get; set; }
        public string UnitOfMeasurement { get; set; } // e.g., "KG", "Liter", "Piece"

        // Relationship: One PackageType is used in many ProductPackages
        public ICollection<ProductPackage> ProductPackages { get; set; }
    }
}
