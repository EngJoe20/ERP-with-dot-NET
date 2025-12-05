using ERP_API.DataAccess.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Sales
{
    public class SalesInvoiceItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int UnitCount { get; set; }
        public decimal SellingPrice { get; set; } // Per unit
        public decimal Total { get; set; } // Auto-calculated

        // Invoice Relationship
        public int SalesInvoiceId { get; set; }
        public SalesInvoice SalesInvoice { get; set; } = default!;

        // Product Package Relationship
        public int ProductPackageId { get; set; }
        public ProductPackage ProductPackage { get; set; } = default!;
    }
}
