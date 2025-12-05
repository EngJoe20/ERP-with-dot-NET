using ERP_API.DataAccess.Entities.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.DataAccess.Entities.User;

namespace ERP_API.DataAccess.Entities.Sales
{
    public class SalesReturn
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string? Reason { get; set; }

        // Customer Relationship
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        //public int UserId { get; set; } = default!;

        // Return Items
        public ICollection<SalesReturnItem> Items { get; set; } = new List<SalesReturnItem>();
    }
}
