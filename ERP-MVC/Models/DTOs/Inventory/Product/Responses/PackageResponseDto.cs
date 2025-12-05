using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Inventory.Product.Responses
{
    public class PackageResponseDto
    {
        public int Id { get; set; }
        public string PackageTypeName { get; set; }
        public string Barcode { get; set; }

        public decimal QinP { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }

        public decimal CurrentStock { get; set; }
    }
}
