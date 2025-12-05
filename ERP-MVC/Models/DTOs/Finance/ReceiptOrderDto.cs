using System.ComponentModel.DataAnnotations;

namespace ERP_MVC.Models.DTOs.Finance
{
    // DTO for API Communication - Receipt Order
    public class ReceiptOrderDto
    {
        public int Id { get; set; }
        public int MainSafeId { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public TransactionDirection Direction { get; set; }
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // DTO for Creating Receipt Order
    public class CreateReceiptOrderDto
    {
        public string ReferenceTable { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ExpenseName { get; set; }
        public string? SourceName { get; set; }

        // MUST BE PUBLIC SET
        public string PerformedByUserId { get; set; } = string.Empty;
    }


    // DTO for Updating Receipt Order
    public class UpdateReceiptOrderDto
    {
        public string ReferenceTable { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ExpenseName { get; set; }
        public string? SourceName { get; set; }
    }

    // DTO for Create Data Response
    public class CreateDataDto
    {
        public List<CustomerDto>? Customers { get; set; }
        public List<SupplierDto>? Suppliers { get; set; }
    }

    // Customer DTO
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }

    // Supplier DTO
    public class SupplierDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    public enum TransactionDirection
    {
        In,
        Out
    }
}