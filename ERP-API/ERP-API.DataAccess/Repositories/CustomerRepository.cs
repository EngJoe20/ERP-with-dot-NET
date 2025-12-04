using ERP_API.DataAccess.DataContext;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Interfaces.Customers;
using Microsoft.EntityFrameworkCore;

namespace ERP_API.DataAccess.Repositories.Customers
{
    internal class CustomerRepository
        : BaseRepository<Customer, int>, ICustomerRepository
    {
        private readonly ErpDBContext _context;

        public CustomerRepository(ErpDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerWithTransactionsAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Transactions)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
