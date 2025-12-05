using System;
using System.ComponentModel.DataAnnotations;

namespace ERP_MVC.Models.DTOs.Finance
{
    // Main Safe DTOs
    public class MainSafeDto
    {
        public int Id { get; set; }
        public string SafeName { get; set; } = string.Empty;
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateMainSafeDto
    {
        [Required(ErrorMessage = "Safe name is required")]
        [StringLength(200, ErrorMessage = "Safe name cannot exceed 200 characters")]
        public string SafeName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Opening balance must be non-negative")]
        public decimal OpeningBalance { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateMainSafeDto
    {
        [StringLength(200, ErrorMessage = "Safe name cannot exceed 200 characters")]
        public string? SafeName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Opening balance must be non-negative")]
        public decimal? OpeningBalance { get; set; }

        public bool? IsActive { get; set; }
    }

    // Ledger Entry DTOs
    public class MainSafeLedgerEntryDto
    {
        public int Id { get; set; }
        public int MainSafeId { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public string ReferenceTable { get; set; } = string.Empty;
        public int ReferenceRecordId { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string PerformedByUserId { get; set; } = string.Empty;
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Related entity names
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string? ExpenseName { get; set; }
        public string? ProfitSourceName { get; set; }
    }

    public class CreateMainSafeLedgerEntryDto
    {
        [Required(ErrorMessage = "Main Safe ID is required")]
        public int MainSafeId { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? EntryDescription { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Debit amount must be non-negative")]
        public decimal DebitAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Credit amount must be non-negative")]
        public decimal CreditAmount { get; set; }

        [Required(ErrorMessage = "Reference table is required")]
        [StringLength(200)]
        public string ReferenceTable { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reference record ID is required")]
        public int ReferenceRecordId { get; set; }

        [Required(ErrorMessage = "Direction is required")]
        public string Direction { get; set; } = string.Empty; // "In" or "Out"
    }

    public class UpdateMainSafeLedgerEntryDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? EntryDescription { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Debit amount must be non-negative")]
        public decimal DebitAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Credit amount must be non-negative")]
        public decimal CreditAmount { get; set; }

        [Required(ErrorMessage = "Direction is required")]
        public string Direction { get; set; } = string.Empty;
    }

    public class MainSafeLedgerEntryDetailsDto
    {
        public int Id { get; set; }
        public int MainSafeId { get; set; }
        public string MainSafeName { get; set; } = string.Empty;
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public string ReferenceTable { get; set; } = string.Empty;
        public int ReferenceRecordId { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string PerformedByUserId { get; set; } = string.Empty;
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Related entity details
        public CustomerTransactionInfo? CustomerTransaction { get; set; }
        public SupplierTransactionInfo? SupplierTransaction { get; set; }
        public ExpenseInfo? Expense { get; set; }
        public ProfitSourceInfo? ProfitSource { get; set; }
    }

    public class CustomerTransactionInfo
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }

    public class SupplierTransactionInfo
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }

    public class ExpenseInfo
    {
        public int Id { get; set; }
        public string ExpenseName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class ProfitSourceInfo
    {
        public int Id { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class MainSafeLedgerEntryFilterDto
    {
        public int? MainSafeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Direction { get; set; }
        public string? ReferenceTable { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }

    public class MainSafeLedgerSummaryDto
    {
        public int TotalEntries { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
    }
}