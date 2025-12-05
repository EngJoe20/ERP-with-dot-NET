using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseReturn
{
    public class PurchaseReturnItemDto
    {
        public int ProductPackageId { get; set; } 
        public int Quantity { get; set; } 
        public int UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        // total auto calculated
    }
}
