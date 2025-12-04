using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseReturn
{
    public class CreatePurchaseReturnDto
    {
        public Guid SupplierId { get; set; } 
        public DateTime ReturnDate { get; set; } //تاريخ المرتجع
        public string? Reason { get; set; }
        public List<PurchaseReturnItemDto> Items { get; set; } = new();
    }
}
