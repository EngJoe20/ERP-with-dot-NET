using ERP_API.DataAccess.Entities.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.DataAccess.Entities.User;

namespace ERP_API.DataAccess.Entities.Purchasing
{
    public class PurchaseReturn
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string? Reason { get; set; }

        // Supplier Relationship
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = default!;

        //public int UserId { get; set; } = default!;

        // Return Items
        public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
    }
}
