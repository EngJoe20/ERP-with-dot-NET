using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Product
{
    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // ✅ NEW: The count
        public int VariationCount { get; set; }
    }
}
