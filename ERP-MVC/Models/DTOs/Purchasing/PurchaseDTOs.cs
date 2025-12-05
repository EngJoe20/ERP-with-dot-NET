using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Purchasing
{
    public class CreatePurchaseInvoiceDto
    {
        public int SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? PaymentOrderAmount { get; set; } //مبلغ امر دفع اختياري
        public List<PurchaseInvoiceItemDto> Items { get; set; } = new();
    }

    public class PurchaseInvoiceItemDto
    {
        public int ProductPackageId { get; set; }
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PurchaseInvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = default!;
        public DateTime InvoiceDate { get; set; }
        public string SupplierName { get; set; } = default!;
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal? PaymentOrderAmount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUser { get; set; } = default!;
        public List<PurchaseInvoiceItemResponseDto> Items { get; set; } = new();
    }

    public class PurchaseInvoiceItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class PurchaseInvoiceListItemDto
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public string SupplierName { get; set; } = default!;
        public string InvoiceNumber { get; set; } = default!;
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
    }

    // ============= Purchase Return DTOs =============

    public class CreatePurchaseReturnDto
    {
        public int SupplierId { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.Now;
        public string? Reason { get; set; }
        public List<PurchaseReturnItemDto> Items { get; set; } = new();
    }

    public class PurchaseReturnItemDto
    {
        public int ProductPackageId { get; set; }
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PurchaseReturnResponseDto
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public string SupplierName { get; set; } = default!;
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Reason { get; set; }
        public List<PurchaseReturnItemResponseDto> Items { get; set; } = new();
    }

    public class PurchaseReturnItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class PurchaseReturnListItemDto
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public string SupplierName { get; set; } = default!;
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}