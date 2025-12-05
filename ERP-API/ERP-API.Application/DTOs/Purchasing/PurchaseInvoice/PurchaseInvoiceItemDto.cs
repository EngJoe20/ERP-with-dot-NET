using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseInvoice
{
    public class PurchaseInvoiceItemDto
    {
        public int ProductPackageId { get; set; } //كود المنتج
        public int Quantity { get; set; } //الكمية
        public int UnitCount { get; set; } //عدد الوحدات
        public decimal UnitPrice { get; set; } //سعر الوحدة
        //total = Quantity* UnitCount*UnitPrice -> auto-calculated
    }
}
