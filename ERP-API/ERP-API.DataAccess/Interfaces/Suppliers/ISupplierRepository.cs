using ERP_API.DataAccess.Entities.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Interfaces.Suppliers
{
    public interface ISupplierRepository : IBaseRepository<Supplier, int>
    {
        Task<Supplier?> GetSupplierWithTransactionsAsync(int id);
    }
}
