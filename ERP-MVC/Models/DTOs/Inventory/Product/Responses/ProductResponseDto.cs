using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Inventory.Product.Responses
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Flattened Category (Simple String, not an object)
        public string? CategoryName { get; set; }

        // List of Variations (Simple DTOs, not Entities)
        public List<VariationResponseDto> Variations { get; set; }
    }
}
