using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Purchasing.PurchaseInvoice
{
    public class PurchaseInvoiceItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!; //كود
        public string ProductName { get; set; } = default!; //المنتج
        public string PackageTypeName { get; set; } = string.Empty;
        public int Quantity { get; set; } //الكمية
        public int UnitCount { get; set; } //عدد الوحدات
        public decimal UnitPrice { get; set; } //سعر الوحدة
        public decimal Total { get; set; } //المجموع
    }
}
