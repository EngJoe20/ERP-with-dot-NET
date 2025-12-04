using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateCustomerDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
    }
    public class CustomerTransactionDto
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Direction { get; set; }
        public string? Description { get; set; }
    }

    public class CustomerDetailsDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalBalance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<CustomerTransactionDto> Transactions { get; set; } = new();
    }
}
