using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesReturn
{
    public class SalesReturnListItemDto
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public string CustomerName { get; set; } = default!; 
        public DateTime ReturnDate { get; set; } 
        public decimal TotalAmount { get; set; } 
    }
}
