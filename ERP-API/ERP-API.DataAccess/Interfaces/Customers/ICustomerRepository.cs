using ERP_API.DataAccess.Entities.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Interfaces.Customers
{
    public interface ICustomerRepository : IBaseRepository<Customer, int>
    {
        Task<Customer?> GetCustomerWithTransactionsAsync(int id);
    }
}
