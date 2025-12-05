using ERP_API.DataAccess.Entities.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.DataAccess.Entities.User;


namespace ERP_API.DataAccess.Entities.Sales
{
    public class SalesInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = default!; // Auto-generated
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? AmountReceived { get; set; } // Partial prepayment
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }

        // Customer Relationship
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        //public int UserId { get; set; } = default!;


        // Invoice Items
        public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
