using ERP_API.Application.DTOs.Customers;
using ERP_API.Application.Interfaces.Customers;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (customer == null) return null;

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
    }
}
