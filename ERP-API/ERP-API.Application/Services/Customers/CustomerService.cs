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

        // ============================================================
        // 1. Get All Customers
        // ============================================================
        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();

            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                CustomerName = c.CustomerName,
                TaxNumber = c.TaxNumber,
                Email = c.Email,
                Phone = c.Phone,
                OpeningBalance = c.OpeningBalance,
                TotalBalance = c.TotalBalance,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        // ============================================================
        // 2. Get Customer By ID
        // ============================================================
        public async Task<CustomerDto?> GetCustomerAsync(int id)
        {
            var customer = await _unitOfWork.Customers.FindByIdAsync(id);
            if (customer == null) return null;

            return new CustomerDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                TaxNumber = customer.TaxNumber,
                Email = customer.Email,
                Phone = customer.Phone,
                OpeningBalance = customer.OpeningBalance,
                TotalBalance = customer.TotalBalance,
                Description = customer.Description,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }

        // ============================================================
        // 3. Get Customer With Transactions
        // ============================================================
        public async Task<CustomerDetailsDto?> GetCustomerDetailsAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithTransactionsAsync(id);
            if (customer == null) return null;

            return new CustomerDetailsDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                TaxNumber = customer.TaxNumber,
                Email = customer.Email,
                Phone = customer.Phone,
                OpeningBalance = customer.OpeningBalance,
                TotalBalance = customer.TotalBalance,
                Description = customer.Description,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                Transactions = customer.Transactions.Select(t => new CustomerTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.CustomerTransactionType,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    Direction = t.Direction,
                    Description = t.Description
                }).ToList()
            };
        }

        // ============================================================
        // 4. Get Customer Transactions Only
        // ============================================================
        public async Task<IEnumerable<CustomerTransactionDto>> GetCustomerTransactionsAsync(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithTransactionsAsync(customerId);
            if (customer == null)
                return Enumerable.Empty<CustomerTransactionDto>();

            return customer.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new CustomerTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.CustomerTransactionType,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    Direction = t.Direction,
                    Description = t.Description
                });
        }

        // ============================================================
        // 5. Create Customer
        // ============================================================
        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var entity = new Customer
            {
                CustomerName = dto.CustomerName,
                TaxNumber = dto.TaxNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                OpeningBalance = dto.OpeningBalance,
                TotalBalance = dto.TotalBalance,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Customers.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerDto
            {
                Id = entity.Id,
                CustomerName = entity.CustomerName,
                TaxNumber = entity.TaxNumber,
                Email = entity.Email,
                Phone = entity.Phone,
                OpeningBalance = entity.OpeningBalance,
                TotalBalance = entity.TotalBalance,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // ============================================================
        // 6. Update Customer
        // ============================================================
        public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            var c = await _unitOfWork.Customers.FindByIdAsync(id);
            if (c == null) return null;

            c.CustomerName = dto.CustomerName;
            c.TaxNumber = dto.TaxNumber;
            c.Email = dto.Email;
            c.Phone = dto.Phone;
            c.OpeningBalance = dto.OpeningBalance;
            c.TotalBalance = dto.TotalBalance;
            c.Description = dto.Description;
            c.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(c);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerDto
            {
                Id = c.Id,
                CustomerName = c.CustomerName,
                TaxNumber = c.TaxNumber,
                Email = c.Email,
                Phone = c.Phone,
                OpeningBalance = c.OpeningBalance,
                TotalBalance = c.TotalBalance,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }

        // ============================================================
        // 7. Delete Customer
        // ============================================================
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var deleted = await _unitOfWork.Customers.DeleteAsync(id);
            if (deleted == null) return false;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // 8. Recalculate Balance
        // ============================================================
        public async Task RecalculateCustomerBalanceAsync(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithTransactionsAsync(customerId);
            if (customer == null) return;

            decimal balance = customer.OpeningBalance; 

            foreach (var t in customer.Transactions.OrderBy(t => t.TransactionDate))
            {
                if (t.Direction == CustomerTransactionDirection.In)
                    balance += t.Amount;
                else
                    balance -= t.Amount;
            }

            customer.TotalBalance = balance;
            customer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
