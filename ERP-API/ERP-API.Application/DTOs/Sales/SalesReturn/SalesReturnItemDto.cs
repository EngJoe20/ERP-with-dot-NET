using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesReturn
{
    public class SalesReturnItemDto
    {
        public int ProductPackageId { get; set; } 
        public int Quantity { get; set; } 
        public int UnitCount { get; set; }
        public decimal Price { get; set; }
    }
}
