using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Finance
{
    [Table("MainSafeLedgerEntries")]
    public class MainSafeLedgerEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MainSafeId { get; set; }

        [Required]
        public DateTime EntryTimestamp { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? EntryDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfterEntry { get; set; }

        [Required]
        [StringLength(200)]
        public string ReferenceTable { get; set; } = string.Empty;

        [Required]
        public int ReferenceRecordId { get; set; }

        public string? PerformedByUserId { get; set; }

        [Required]
        public TransactionDirection Direction { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("MainSafeId")]
        public virtual MainSafe MainSafe { get; set; } = null!;

        [ForeignKey(nameof(PerformedByUserId))]
        public AppUser? PerformedByUser { get; set; }

        // Polymorphic relationships (handled differently in EF Core)
        public virtual CustomerTransaction? CustomerTransaction { get; set; }
        public virtual SupplierTransaction? SupplierTransaction { get; set; }
        public virtual Expense? Expense { get; set; }
        public virtual ProfitSource? ProfitSource { get; set; }
      
    }

    public enum TransactionDirection
    {
        In,
        Out
    }
}
