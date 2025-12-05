using System;
using System.Collections.Generic;

namespace ERP_MVC.Models.DTOs.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateCustomerDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }

    public class CustomerTransactionDto
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } = string.Empty; // 'Payment', 'Receipt', etc.
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Direction { get; set; } = string.Empty; // 'In' or 'Out'
        public string? Description { get; set; }
    }

    public class CustomerDetailsDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<CustomerTransactionDto> Transactions { get; set; } = new();
    }
}
