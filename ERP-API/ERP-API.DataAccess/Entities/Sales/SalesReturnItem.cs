using ERP_API.DataAccess.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Sales
{
    public class SalesReturnItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }

        // Return Relationship
        public int SalesReturnId { get; set; }
        public SalesReturn SalesReturn { get; set; } = default!;

        // Product Package Relationship
        public int ProductPackageId { get; set; }
        public ProductPackage ProductPackage { get; set; } = default!;
    }
}
