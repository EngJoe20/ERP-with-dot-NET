using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Finance;
using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.InventoryAdjustment;
using ERP_API.DataAccess.Entities.Purchasing;
using ERP_API.DataAccess.Entities.Sales;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Entities.Warehouse;
using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.Interfaces.Customers;
using ERP_API.DataAccess.Interfaces.Suppliers;
using ERP_API.DataAccess.Repositories;
using ERP_API.DataAccess.Repositories.Customers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.DataContext
{
  

    namespace ERP_API.DataAccess.DataContext
    {
        internal class ErpUnitOfWork : IErpUnitOfWork
        {
            private readonly ErpDBContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenManager _tokenManager;


            // 1. Define Lazy fields for all repositories
            private readonly Lazy<IBaseRepository<Product, int>> _products;
            private readonly Lazy<IBaseRepository<ProductVariation, int>> _productVariations;
            private readonly Lazy<IBaseRepository<ProductPackage, int>> _productPackages;
            private readonly Lazy<IBaseRepository<PackageType, int>> _packageTypes;
            private readonly Lazy<IBaseRepository<Warehouse, int>> _warehouses;
            private readonly Lazy<IBaseRepository<WarehouseStock, int>> _warehouseStocks;
            private readonly Lazy<IBaseRepository<StockTransferLog, int>> _stockTransferLogs;
            private readonly Lazy<IBaseRepository<InventoryAdjustment, int>> _inventoryAdjustments;



            // EngJoe
            private readonly Lazy<IBaseRepository<MainSafe, int>> _mainSafes;
            private readonly Lazy<IBaseRepository<MainSafeLedgerEntry, int>> _mainSafeLedgerEntries;
            private readonly Lazy<IBaseRepository<Expense, int>> _expenses;
            private readonly Lazy<IBaseRepository<ProfitSource, int>> _profitSources;
            private readonly Lazy<IBaseRepository<CustomerTransaction, int>> _customerTransactions;

            private readonly Lazy<IBaseRepository<SupplierTransaction, int>> _supplierTransactions;

            private readonly Lazy<IBaseRepository<ReceiptOrder, int>> _receiptOrder;
            private readonly Lazy<IBaseRepository<PaymentOrder, int>> _paymentOrder;

            private readonly Lazy<ICustomerRepository> _customers;
            private readonly Lazy<ISupplierRepository> _suppliers;



            // AFNAN
            //purchasing and sales
            private IBaseRepository<PurchaseInvoice, int>? _purchaseInvoices;
            private IBaseRepository<PurchaseInvoiceItem, int>? _purchaseInvoiceItems;
            private IBaseRepository<PurchaseReturn, int>? _purchaseReturns;
            private IBaseRepository<PurchaseReturnItem, int>? _purchaseReturnItems;
            private IBaseRepository<SalesInvoice, int>? _salesInvoices;
            private IBaseRepository<SalesInvoiceItem, int>? _salesInvoiceItems;
            private IBaseRepository<SalesReturn, int>? _salesReturns;
            private IBaseRepository<SalesReturnItem, int>? _salesReturnItems;


            // 2. Constructor: Inject Context and Initialize Lazies
            public ErpUnitOfWork(ErpDBContext context, UserManager<AppUser> userManager, ITokenManager tokenManager)
            {
                _context = context;
                _userManager = userManager;
                _tokenManager = tokenManager;

                // Note: We use () => new ... (Lambda expression)
                // This ensures the object is ONLY created when someone asks for .Value

                _products = new Lazy<IBaseRepository<Product, int>>(() =>
                    new BaseRepository<Product, int>(_context));

                _productVariations = new Lazy<IBaseRepository<ProductVariation, int>>(() =>
                    new BaseRepository<ProductVariation, int>(_context));

                _productPackages = new Lazy<IBaseRepository<ProductPackage, int>>(() =>
                    new BaseRepository<ProductPackage, int>(_context));

                _packageTypes = new Lazy<IBaseRepository<PackageType, int>>(() =>
                    new BaseRepository<PackageType, int>(_context));

                _warehouses = new Lazy<IBaseRepository<Warehouse, int>>(() =>
                    new BaseRepository<Warehouse, int>(_context));

                _warehouseStocks = new Lazy<IBaseRepository<WarehouseStock, int>>(() =>
                    new BaseRepository<WarehouseStock, int>(_context));

                _stockTransferLogs = new Lazy<IBaseRepository<StockTransferLog, int>>(() =>
                    new BaseRepository<StockTransferLog, int>(_context));

                _inventoryAdjustments = new Lazy<IBaseRepository<InventoryAdjustment, int>>(() =>
                    new BaseRepository<InventoryAdjustment, int>(_context));

                // EngJoe
                _mainSafes = new Lazy<IBaseRepository<MainSafe, int>>(() =>
                    new BaseRepository<MainSafe, int>(_context));
                _mainSafeLedgerEntries = new Lazy<IBaseRepository<MainSafeLedgerEntry, int>>(() =>
                    new BaseRepository<MainSafeLedgerEntry, int>(_context));
                _expenses = new Lazy<IBaseRepository<Expense, int>>(() =>
                    new BaseRepository<Expense, int>(_context));
                _profitSources = new Lazy<IBaseRepository<ProfitSource, int>>(() =>
                    new BaseRepository<ProfitSource, int>(_context));
                _customerTransactions = new Lazy<IBaseRepository<CustomerTransaction, int>>(() =>
                new BaseRepository<CustomerTransaction, int>(_context));

                _supplierTransactions = new Lazy<IBaseRepository<SupplierTransaction, int>>(() =>
                    new BaseRepository<SupplierTransaction, int>(_context));



                _receiptOrder = new Lazy<IBaseRepository<ReceiptOrder, int>>(() =>
                     new BaseRepository<ReceiptOrder, int>(_context));

                _paymentOrder = new Lazy<IBaseRepository<PaymentOrder, int>>(() =>
                    new BaseRepository<PaymentOrder, int>(_context));

                _customers = new Lazy<ICustomerRepository>(() =>
                    new CustomerRepository(_context));

                _suppliers = new Lazy<ISupplierRepository>(() =>
                   new SupplierRepository(_context));
            }





            public IBaseRepository<PurchaseInvoice, int> PurchaseInvoices =>
            _purchaseInvoices ??= new BaseRepository<PurchaseInvoice, int>(_context);
            public IBaseRepository<PurchaseInvoiceItem, int> PurchaseInvoiceItems =>
                _purchaseInvoiceItems ??= new BaseRepository<PurchaseInvoiceItem, int>(_context);
            public IBaseRepository<PurchaseReturn, int> PurchaseReturns =>
                _purchaseReturns ??= new BaseRepository<PurchaseReturn, int>(_context);
            public IBaseRepository<PurchaseReturnItem, int> PurchaseReturnItems =>
                _purchaseReturnItems ??= new BaseRepository<PurchaseReturnItem, int>(_context);

            //Sales
            public IBaseRepository<SalesInvoice, int> SalesInvoices =>
                _salesInvoices ??= new BaseRepository<SalesInvoice, int>(_context);
            public IBaseRepository<SalesInvoiceItem, int> SalesInvoiceItems =>
                _salesInvoiceItems ??= new BaseRepository<SalesInvoiceItem, int>(_context);
            public IBaseRepository<SalesReturn, int> SalesReturns =>
                _salesReturns ??= new BaseRepository<SalesReturn, int>(_context);
            public IBaseRepository<SalesReturnItem, int> SalesReturnItems =>
                _salesReturnItems ??= new BaseRepository<SalesReturnItem, int>(_context);




            

            // 3. Public Properties return the .Value
            public IBaseRepository<Product, int> Products => _products.Value;
            public IBaseRepository<ProductVariation, int> ProductVariations => _productVariations.Value;
            public IBaseRepository<ProductPackage, int> ProductPackages => _productPackages.Value;
            public IBaseRepository<PackageType, int> PackageTypes => _packageTypes.Value;
            public IBaseRepository<Warehouse, int> Warehouses => _warehouses.Value;
            public IBaseRepository<WarehouseStock, int> WarehouseStocks => _warehouseStocks.Value;
            public IBaseRepository<StockTransferLog, int> StockTransferLogs => _stockTransferLogs.Value;
            public IBaseRepository<InventoryAdjustment, int> InventoryAdjustments => _inventoryAdjustments.Value;


            // EngJoe
            public IBaseRepository<MainSafe, int> MainSafes => _mainSafes.Value;
            public IBaseRepository<MainSafeLedgerEntry, int> MainSafeLedgerEntry => _mainSafeLedgerEntries.Value;
            public IBaseRepository<Expense, int> Expenses => _expenses.Value;
            public IBaseRepository<ProfitSource, int> ProfitSources => _profitSources.Value;
            public IBaseRepository<ReceiptOrder, int> ReceiptOrder => _receiptOrder.Value;
            public IBaseRepository<PaymentOrder, int> PaymentOrder => _paymentOrder.Value;

            public ICustomerRepository Customers => _customers.Value;
            public ISupplierRepository Suppliers => _suppliers.Value;

            public IBaseRepository<CustomerTransaction, int> CustomerTransactions => _customerTransactions.Value;
            public IBaseRepository<SupplierTransaction, int> SupplierTransactions => _supplierTransactions.Value;




            public UserManager<AppUser> UserManager => _userManager;

            public ITokenManager TokenManager => _tokenManager;


            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
    }

}
