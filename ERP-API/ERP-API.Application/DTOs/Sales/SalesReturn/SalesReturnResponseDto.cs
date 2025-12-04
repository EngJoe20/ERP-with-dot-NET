using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesReturn
{
    public class SalesReturnResponseDto
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public string CustomerName { get; set; } = default!; 
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; } 
        public string? Reason { get; set; }
        public List<SalesReturnItemResponseDto> Items { get; set; } = new();
    }
}
