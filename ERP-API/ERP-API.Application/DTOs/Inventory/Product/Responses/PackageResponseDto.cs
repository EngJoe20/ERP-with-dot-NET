using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Product.Responses
{
    public class PackageResponseDto
    {
        public int Id { get; set; }
        public required string PackageTypeName { get; set; } // "Carton"
        public decimal QinP { get; set; }
        public decimal SalesPrice { get; set; }
        public required string Barcode { get; set; }
    }
}
