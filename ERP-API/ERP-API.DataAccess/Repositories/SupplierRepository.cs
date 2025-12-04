using ERP_API.DataAccess.DataContext;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Interfaces.Suppliers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Repositories
{
    internal class SupplierRepository
        : BaseRepository<Supplier, int>, ISupplierRepository
    {
        private readonly ErpDBContext _context;

        public SupplierRepository(ErpDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetSupplierWithTransactionsAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Transactions)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
