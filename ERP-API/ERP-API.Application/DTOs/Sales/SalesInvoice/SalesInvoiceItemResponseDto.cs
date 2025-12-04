using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesInvoice
{
    public class SalesInvoiceItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!; 
        public string ProductName { get; set; } = default!;
        public string PackageTypeName { get; set; } = string.Empty;
        public int Quantity { get; set; } 
        public int UnitCount { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Total { get; set; }
    }
}
