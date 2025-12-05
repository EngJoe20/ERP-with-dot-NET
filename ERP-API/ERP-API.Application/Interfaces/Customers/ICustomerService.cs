using ERP_API.Application.DTOs.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Customers
{
    public interface ICustomerService
    {
        // Read operations
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerAsync(int id);
        Task<CustomerDetailsDto?> GetCustomerDetailsAsync(int id);
        Task<IEnumerable<CustomerTransactionDto>> GetCustomerTransactionsAsync(int customerId);

        // Write operations
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto);
        Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(int id);

        // Helper methods
        Task RecalculateCustomerBalanceAsync(int customerId);
    }
}
