
using System.ComponentModel.DataAnnotations;

namespace ERP_MVC.Models.ViewModels.Finance
{
    // ViewModel for displaying receipt orders in list
    public class ReceiptOrderViewModel
    {
        public int Id { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string PartyName => CustomerName ?? SupplierName ?? "N/A";
        public string FormattedDate => EntryTimestamp.ToString("MM/dd/yyyy");
        public string FormattedAmount => CreditAmount.ToString("N2");
    }

    // ViewModel for creating receipt orders
    public class CreateReceiptOrderViewModel
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
        public string PerformedByUserId { get; set; }

        // For dropdown population
        public List<ERP_MVC.Models.DTOs.Finance.CustomerDto> Customers { get; set; } = new();
        public List<ERP_MVC.Models.DTOs.Finance.SupplierDto> Suppliers { get; set; } = new();
    }

    // ViewModel for updating receipt orders
    public class UpdateReceiptOrderViewModel
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
        public List<ERP_MVC.Models.DTOs.Finance.CustomerDto> Customers { get; set; } = new();
        public List<ERP_MVC.Models.DTOs.Finance.SupplierDto> Suppliers { get; set; } = new();
    }
}