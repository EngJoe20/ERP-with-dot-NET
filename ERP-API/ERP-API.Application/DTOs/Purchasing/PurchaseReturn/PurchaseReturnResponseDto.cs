using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseReturn
{
    public class PurchaseReturnResponseDto
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public string SupplierName { get; set; } = default!;
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; } //المبلغ
        public string? Reason { get; set; }
        public List<PurchaseReturnItemResponseDto> Items { get; set; } = new();
    }
}
