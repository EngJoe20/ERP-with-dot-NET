using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.InventoryAdjustment;
using ERP_API.DataAccess.Entities.Warehouse;
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

        Task SaveChangesAsync();
    }
}
