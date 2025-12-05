namespace ERP_MVC.Models.DTOs.Sales
{
    // ============= Sales Invoice DTOs =============

    public class CreateSalesInvoiceDto
    {
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public decimal? Discount { get; set; }
        public decimal? AmountReceived { get; set; }
        public List<SalesInvoiceItemDto> Items { get; set; } = new();
    }

    public class SalesInvoiceItemDto
    {
        public int ProductPackageId { get; set; }
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal SellingPrice { get; set; }
    }

    public class SalesInvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = default!;
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; } = default!;
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal? AmountReceived { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUser { get; set; } = default!;
        public List<SalesInvoiceItemResponseDto> Items { get; set; } = new();
    }

    public class SalesInvoiceItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class SalesInvoiceListItemDto
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public string CustomerName { get; set; } = default!;
        public string InvoiceNumber { get; set; } = default!;
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
    }

    // ============= Sales Return DTOs =============

    public class CreateSalesReturnDto
    {
        public int CustomerId { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.Now;
        public string? Reason { get; set; }
        public List<SalesReturnItemDto> Items { get; set; } = new();
    }

    public class SalesReturnItemDto
    {
        public int ProductPackageId { get; set; }
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal Price { get; set; }
    }

    public class SalesReturnResponseDto
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public string CustomerName { get; set; } = default!;
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Reason { get; set; }
        public List<SalesReturnItemResponseDto> Items { get; set; } = new();
    }

    public class SalesReturnItemResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public class SalesReturnListItemDto
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public string CustomerName { get; set; } = default!;
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}