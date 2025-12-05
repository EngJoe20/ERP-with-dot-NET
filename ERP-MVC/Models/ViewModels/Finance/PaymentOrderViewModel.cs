using System.ComponentModel.DataAnnotations;
using ERP_MVC.Models.DTOs.Finance;

namespace ERP_MVC.Models.ViewModels.Finance
{
    // ViewModel for displaying payment orders in list
    public class PaymentOrderViewModel
    {
        public int Id { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }

        // Payment = Debit
        public decimal DebitAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string PartyName => CustomerName ?? SupplierName ?? "N/A";
        public string FormattedDate => EntryTimestamp.ToString("MM/dd/yyyy");
        public string FormattedAmount => DebitAmount.ToString("N2");
    }

    // ViewModel for creating payment orders
    public class CreatePaymentOrderViewModel
    {
        [Required(ErrorMessage = "Reference type is required")]
        public string? ReferenceTable { get; set; }

        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(200, ErrorMessage = "Expense name cannot exceed 200 characters")]
        public string? ExpenseName { get; set; }

        [StringLength(200, ErrorMessage = "Source name cannot exceed 200 characters")]
        public string? SourceName { get; set; }
        public string PerformedByUserId { get; set; } = string.Empty;

        // For dropdown population
        public List<CustomerDto> Customers { get; set; } = new();
        public List<SupplierDto> Suppliers { get; set; } = new();
    }

    // ViewModel for updating payment orders
    public class UpdatePaymentOrderViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Reference type is required")]
        public string? ReferenceTable { get; set; }

        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(200, ErrorMessage = "Expense name cannot exceed 200 characters")]
        public string? ExpenseName { get; set; }

        [StringLength(200, ErrorMessage = "Source name cannot exceed 200 characters")]
        public string? SourceName { get; set; }

        // For dropdown population
        public List<CustomerDto> Customers { get; set; } = new();
        public List<SupplierDto> Suppliers { get; set; } = new();
    }
}
