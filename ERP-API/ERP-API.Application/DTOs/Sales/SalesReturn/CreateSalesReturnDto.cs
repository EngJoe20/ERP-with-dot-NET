using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesReturn
{
    public class CreateSalesReturnDto
    {
        public int CustomerId { get; set; } 
        public DateTime ReturnDate { get; set; } 
        public string? Reason { get; set; }
        public List<SalesReturnItemDto> Items { get; set; } = new();
    }
}