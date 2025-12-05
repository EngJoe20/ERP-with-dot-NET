using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseInvoice
{
    public class CreatePurchaseInvoiceDto
    {
        public int SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? PaymentOrderAmount { get; set; } //مبلغ امر دفع اختياري
        public List<PurchaseInvoiceItemDto> Items { get; set; } = new();
    }
}
