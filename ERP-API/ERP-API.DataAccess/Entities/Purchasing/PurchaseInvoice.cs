using ERP_API.DataAccess.Entities.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.DataAccess.Entities.User;

namespace ERP_API.DataAccess.Entities.Purchasing
{
    public class PurchaseInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = default!; // Auto-generated
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? PaymentOrderAmount { get; set; } // Partial payment
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }

        // Supplier Relationship
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = default!;

        //public int UserId { get; set; } = default!;

        // Invoice Items
        public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
