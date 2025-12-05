using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseInvoice
{
    public class PurchaseInvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = default!; //رقم فاتورة المورد تلقائي
        public DateTime InvoiceDate { get; set; } //تاريخ الفاتورة
        public string SupplierName { get; set; } = default!; //المورد
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; } //المبلغ الاجمالي
        public decimal Discount { get; set; } //الخصم
        public decimal NetAmount { get; set; } //المبلغ الصافي
        public decimal BalanceBefore { get; set; } //الرصيد قبل الفاتورة
        public decimal BalanceAfter { get; set; } //الرصيد بعد الفاتورة
        public decimal? PaymentOrderAmount { get; set; } //مبلغ امر الدفع اختياري
        public DateTime CreatedDate { get; set; }
        public string CreatedByUser { get; set; } = default!;
        public List<PurchaseInvoiceItemResponseDto> Items { get; set; } = new();
    }
}
