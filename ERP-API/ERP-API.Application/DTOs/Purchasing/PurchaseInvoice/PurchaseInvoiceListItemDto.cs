using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseInvoice
{
    public class PurchaseInvoiceListItemDto
    {
        public int Id { get; set; } //رقم الفاتورة (row num)
        public string SupplierName { get; set; } = default!;
        public string InvoiceNumber { get; set; } = default!; //رقم فاتورة المورد
        public DateTime InvoiceDate { get; set; } 
        public decimal TotalAmount { get; set; } 
        public decimal Discount { get; set; } 
        public decimal NetAmount { get; set; } //المبلغ الصافي
    }
}
