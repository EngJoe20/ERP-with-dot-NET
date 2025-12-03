using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Finance
{
    public class MainSafeLedgerEntryDto
    {
        public int Id { get; set; }
        public int MainSafeId { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public string? EntryDescription { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAfterEntry { get; set; }
        public string ReferenceTable { get; set; } = string.Empty;
        public int ReferenceRecordId { get; set; }
        public int PerformedByUserId { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string PerformedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class LedgerEntryWithDetailsDto : MainSafeLedgerEntryDto
    {
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string? ReferenceTypeName { get; set; }
    }
}
