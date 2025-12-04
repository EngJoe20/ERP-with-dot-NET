using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseReturn
{
    public class PurchaseReturnListItemDto
    {
        public int RowNumber { get; set; } 
        public string SupplierName { get; set; } = default!; 
        public DateTime ReturnDate { get; set; } 
        public decimal TotalAmount { get; set; } 
    }
}
