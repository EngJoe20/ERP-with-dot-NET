using ERP_API.DataAccess.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Purchasing
{
    public class PurchaseInvoiceItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; } // Per unit
        public decimal Total { get; set; } // Auto-calculated

        // Invoice Relationship
        public int PurchaseInvoiceId { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; } = default!;

        // Product Package Relationship
        public int ProductPackageId { get; set; }
        public ProductPackage ProductPackage { get; set; } = default!;
    }
}
