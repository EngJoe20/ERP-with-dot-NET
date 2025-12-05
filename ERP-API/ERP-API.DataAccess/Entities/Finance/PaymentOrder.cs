using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Entities.User;

namespace ERP_API.DataAccess.Entities.Finance
{
    public class PaymentOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MainSafeId { get; set; }

        [Required]
        public DateTime EntryTimestamp { get; set; }

        [MaxLength(500)]
        public string? EntryDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }   

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfterEntry { get; set; }

        [Required]
        public TransactionDirection Direction { get; set; }

        // Customer
        public int? CustomerTransactionId { get; set; }
        [ForeignKey(nameof(CustomerTransactionId))]
        public CustomerTransaction? CustomerTransaction { get; set; }

        // Supplier
        public int? SupplierTransactionId { get; set; }
        [ForeignKey(nameof(SupplierTransactionId))]
        public SupplierTransaction? SupplierTransaction { get; set; }

        // User
        public string? PerformedByUserId { get; set; }
        [ForeignKey(nameof(PerformedByUserId))]
        public AppUser? PerformedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
