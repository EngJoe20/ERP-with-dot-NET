using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.InventoryAdjustment;
using ERP_API.DataAccess.Entities.Warehouse;
using ERP_API.DataAccess.Repositories;
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

            // 1. Define Lazy fields for all repositories
            private readonly Lazy<IBaseRepository<Product, int>> _products;
            private readonly Lazy<IBaseRepository<ProductVariation, int>> _productVariations;
            private readonly Lazy<IBaseRepository<ProductPackage, int>> _productPackages;
            private readonly Lazy<IBaseRepository<PackageType, int>> _packageTypes;
            private readonly Lazy<IBaseRepository<Warehouse, int>> _warehouses;
            private readonly Lazy<IBaseRepository<WarehouseStock, int>> _warehouseStocks;
            private readonly Lazy<IBaseRepository<StockTransferLog, int>> _stockTransferLogs;
            private readonly Lazy<IBaseRepository<InventoryAdjustment, int>> _inventoryAdjustments;

            // 2. Constructor: Inject Context and Initialize Lazies
            public ErpUnitOfWork(ErpDBContext context)
            {
                _context = context;

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
            }

            // 3. Public Properties return the .Value
            public IBaseRepository<Product, int> Products => _products.Value;
            public IBaseRepository<ProductVariation, int> ProductVariations => _productVariations.Value;
            public IBaseRepository<ProductPackage, int> ProductPackages => _productPackages.Value;
            public IBaseRepository<PackageType, int> PackageTypes => _packageTypes.Value;
            public IBaseRepository<Warehouse, int> Warehouses => _warehouses.Value;
            public IBaseRepository<WarehouseStock, int> WarehouseStocks => _warehouseStocks.Value;
            public IBaseRepository<StockTransferLog, int> StockTransferLogs => _stockTransferLogs.Value;
            public IBaseRepository<InventoryAdjustment, int> InventoryAdjustments => _inventoryAdjustments.Value;



            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
    }
    //public class ErpUnitOfWork : IErpUnitOfWork
    //{
    //    public ErpUnitOfWork()
    //    {
    //        // Initialize Product Repositories
    //        Products = new MockBaseRepository<Product, int>();
    //        ProductVariations = new MockBaseRepository<ProductVariation, int>();
    //        ProductPackages = new MockBaseRepository<ProductPackage, int>();
    //        PackageTypes = new MockBaseRepository<PackageType, int>();

    //        Warehouses = new MockBaseRepository<Warehouse, int>();
    //        WarehouseStocks = new MockBaseRepository<WarehouseStock, int>();
    //    }

    //    // Product Properties
    //    public IBaseRepository<Product, int> Products { get; private set; }
    //    public IBaseRepository<ProductVariation, int> ProductVariations { get; private set; }
    //    public IBaseRepository<ProductPackage, int> ProductPackages { get; private set; }
    //    public IBaseRepository<PackageType, int> PackageTypes { get; private set; }


    //    public IBaseRepository<Warehouse, int> Warehouses { get; private set; }
    //    public IBaseRepository<WarehouseStock, int> WarehouseStocks { get; private set; }

    //    public void SaveChanges()
    //    {
    //        // Mock mode - do nothing
    //    }
    //}
}
