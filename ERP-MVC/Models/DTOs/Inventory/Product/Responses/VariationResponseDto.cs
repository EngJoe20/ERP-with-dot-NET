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
        public string Name { get; set; }
        public string Flavor { get; set; }
        public string SKU { get; set; }

        // List of Packages (Simple DTOs)
        public List<PackageResponseDto> Packages { get; set; }
    }
}
