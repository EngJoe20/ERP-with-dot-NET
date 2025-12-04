using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseReturn
{
    public class PurchaseReturnItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!; 
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public int UnitCount { get; set; } 
        public decimal UnitPrice { get; set; } 
        public decimal Total { get; set; } 
    }
}
