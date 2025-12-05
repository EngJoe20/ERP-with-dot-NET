using System;
using System.Collections.Generic;

namespace ERP_MVC.Models.DTOs.Suppliers
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateSupplierDto
    {
        public string SupplierName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateSupplierDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }

    public class SupplierTransactionDto
    {
        public int Id { get; set; }
        public SupplierTransactionType TransactionType { get; set; }  // 'Payment', 'Receipt', etc.
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public SupplierTransactionDirection Direction { get; set; }// 'In' or 'Out'
        public string? Description { get; set; }
    }

    public class SupplierDetailsDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<SupplierTransactionDto> Transactions { get; set; } = new();
    }

    public enum SupplierTransactionType
    {
        Payment,
        Receipt,
        Sale,
        Purchase
    }

    public enum SupplierTransactionDirection
    {
        In,
        Out
    }
}
