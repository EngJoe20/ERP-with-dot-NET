using ERP_API.DataAccess.Entities.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP_API.DataAccess.Entities.Suppliers;

namespace ERP_API.DataAccess.Entities.Suppliers
{
    [Table("SupplierTransactions")]
    public class SupplierTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public SupplierTransactionType SupplierTransactionType { get; set; } // Enum type

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public SupplierTransactionDirection Direction { get; set; } // Enum type

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;

        public virtual ICollection<MainSafeLedgerEntry> LedgerEntries { get; set; } = new List<MainSafeLedgerEntry>();
    }

    public enum SupplierTransactionType
    {
        Payment,
        Receipt,
        Sale,
        Purchase
    }

    public enum SupplierTransactionDirection
    {
        In,
        Out
    }
}
