using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesInvoice
{
    public class SalesInvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = default!; //رقم فاتورة العميل تلقائي
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; } 
        public decimal BalanceBefore { get; set; } //الرصيد قبل الفاتورة
        public decimal BalanceAfter { get; set; } //الرصيد بعد الفاتورة
        public decimal? AmountReceived { get; set; } //المبلغ المستلم اختياري
        public DateTime CreatedDate { get; set; }
        public string CreatedByUser { get; set; } = default!;
        public List<SalesInvoiceItemResponseDto> Items { get; set; } = new();
    }
}
