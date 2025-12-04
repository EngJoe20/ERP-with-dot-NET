using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Suppliers
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class CreateSupplierDto
    {
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateSupplierDto
    {
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }
    public class SupplierTransactionDto
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
    public class SupplierDetailsDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<SupplierTransactionDto> Transactions { get; set; } = new();
    }

}
