using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Finance
{
    public class CreatePaymentOrderDto
    {
        public string ReferenceTable { get; set; } = string.Empty; // customertransactions, suppliertransactions, profitsources, expenses
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ExpenseName { get; set; }
        public string? SourceName { get; set; }
    }

    public class PaymentOrderDto
    {
        public int Id { get; set; }
        public int MainSafeId { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
