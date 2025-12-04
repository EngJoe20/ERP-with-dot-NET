using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Product.Responses
{
    public class VariationResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Flavor { get; set; }
        public required string SKU { get; set; }

        // List of Packages (Simple DTOs)
        public required List<PackageResponseDto> Packages { get; set; }
    }
}
