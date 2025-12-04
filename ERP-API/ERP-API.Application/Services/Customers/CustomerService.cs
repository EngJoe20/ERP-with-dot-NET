using ERP_API.Application.DTOs.Customers;
using ERP_API.Application.Interfaces.Customers;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Interfaces;

namespace ERP_API.Application.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public CustomerService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();

            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                CustomerName = c.CustomerName,
                TotalBalance = c.TotalBalance,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CustomerDto?> GetCustomerAsync(int id)
        {
            var customer = await _unitOfWork.Customers.FindByIdAsync(id);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                TotalBalance = customer.TotalBalance,
                Description = customer.Description,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }

        public async Task<CustomerDetailsDto?> GetCustomerDetailsAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithTransactionsAsync(id);
            if (customer == null)
                return null;

            return new CustomerDetailsDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                TotalBalance = customer.TotalBalance,
                Description = customer.Description,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                Transactions = customer.Transactions.Select(t => new CustomerTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    Direction = t.Direction,
                    Description = t.Description
                }).ToList()
            };
        }

        public async Task<IEnumerable<CustomerTransactionDto>> GetCustomerTransactionsAsync(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithTransactionsAsync(customerId);
            if (customer == null)
                return Enumerable.Empty<CustomerTransactionDto>();

            return customer.Transactions.Select(t => new CustomerTransactionDto
            {
                Id = t.Id,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                Direction = t.Direction,
                Description = t.Description
            });
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var entity = new Customer
            {
                CustomerName = dto.CustomerName,
                TotalBalance = dto.TotalBalance,
                Description = dto.Description
            };

            await _unitOfWork.Customers.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerDto
            {
                Id = entity.Id,
                CustomerName = entity.CustomerName,
                TotalBalance = entity.TotalBalance,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _unitOfWork.Customers.FindByIdAsync(id);
            if (customer == null)
                return null;

            customer.CustomerName = dto.CustomerName;
            customer.TotalBalance = dto.TotalBalance;
            customer.Description = dto.Description;
            customer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                TotalBalance = customer.TotalBalance,
                Description = customer.Description,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var deleted = await _unitOfWork.Customers.DeleteAsync(id);
            if (deleted == null)
                return false;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
