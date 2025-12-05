using ERP_API.DataAccess.Entities.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_API.DataAccess.Entities.Customers
{
    [Table("CustomerTransactions")]
    public class CustomerTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public CustomerTransactionType CustomerTransactionType { get; set; } // Enum type

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public CustomerTransactionDirection Direction { get; set; } // Enum type

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        public virtual ICollection<MainSafeLedgerEntry> LedgerEntries { get; set; } = new List<MainSafeLedgerEntry>();
    }

    public enum CustomerTransactionType
    {
        Payment,
        Receipt,
        Sale,
        Purchase
    }

    public enum CustomerTransactionDirection
    {
        In,
        Out
    }
}
