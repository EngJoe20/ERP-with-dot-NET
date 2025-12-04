using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesReturn
{
    public class SalesReturnItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!; 
        public string ProductName { get; set; } = default!; 
        public int Quantity { get; set; } 
        public int UnitCount { get; set; } 
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
