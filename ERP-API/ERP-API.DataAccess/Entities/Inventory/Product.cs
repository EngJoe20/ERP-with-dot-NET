using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Inventory
{
    public class Product 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        // Relationship: Product belongs to ONE Category
        public int ? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Relationship: Product has MANY Variations
        public ICollection<ProductVariation> Variations { get; set; }
    }
}
