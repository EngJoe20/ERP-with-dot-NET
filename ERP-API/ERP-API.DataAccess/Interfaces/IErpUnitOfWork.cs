
using ERP_API.DataAccess.Entities.Finance;
using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.InventoryAdjustment;
using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Entities.Warehouse;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Suppliers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Interfaces
{
    public interface IErpUnitOfWork
    {
        IBaseRepository<Product, int> Products { get; }
        IBaseRepository<ProductVariation, int> ProductVariations { get; }
        IBaseRepository<ProductPackage, int> ProductPackages { get; }

        IBaseRepository<PackageType, int> PackageTypes { get; }

        IBaseRepository<Warehouse, int> Warehouses { get; }
        IBaseRepository<WarehouseStock, int> WarehouseStocks { get; }

        IBaseRepository<StockTransferLog, int> StockTransferLogs { get; }

        IBaseRepository<InventoryAdjustment, int> InventoryAdjustments { get; }
        UserManager<AppUser> UserManager { get; }



        IBaseRepository<MainSafe, int> MainSafes { get; }
        IBaseRepository<MainSafeLedgerEntry, int> MainSafeLedgerEntry { get; }
        IBaseRepository<Expense, int> Expenses { get; }
        IBaseRepository<ProfitSource, int> ProfitSources { get; }
        IBaseRepository<Customer, int> Customers { get; }
        IBaseRepository<CustomerTransaction, int> CustomerTransactions { get; }
        IBaseRepository<Supplier, int> Suppliers { get; }
        IBaseRepository<SupplierTransaction, int> SupplierTransactions { get; }




        public ITokenManager TokenManager { get; }

        Task SaveChangesAsync();
    }
}
