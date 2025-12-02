using ERP_API.Application.Interfaces;
using ERP_API.Application.DTOs.Warehouse;
using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public WarehouseService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Ensure Main Warehouse exists.
            // Since Constructors cannot be Async, we execute this synchronously here.
            EnsureMainWarehouseExists();
        }

        private void EnsureMainWarehouseExists()
        {
            // We use .Result or Wait() here because we are in a constructor
            var mainExists = _unitOfWork.Warehouses.GetAllQueryable()
                .Any(w => w.IsMainWarehouse);

            if (!mainExists)
            {
                var mainWarehouse = new Warehouse
                {
                    Name = "Main Virtual Warehouse",
                    Location = "System (Virtual)",
                    IsMainWarehouse = true
                };

                // Sync Create/Save
                _unitOfWork.Warehouses.CreateAsync(mainWarehouse);
                _unitOfWork.SaveChangesAsync();
            }
        }

        // 1. ADD WAREHOUSE (Async)
        public async Task<Warehouse> AddWarehouseAsync(WarehouseInsertDto dto)
        {
            var warehouse = new Warehouse
            {
                Name = dto.Name,
                Location = dto.Location,
                IsMainWarehouse = false
            };

            await _unitOfWork.Warehouses.CreateAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return warehouse;
        }

        // 2. GET ALL (Async)
        public async Task<IEnumerable<WarehouseItemDto>> GetAllWarehousesAsync()
        {
            return await _unitOfWork.Warehouses.GetAllQueryable()
                .Select(w => new WarehouseItemDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    IsMainWarehouse = w.IsMainWarehouse
                })
                .ToListAsync(); 
        }

        // 3. TRANSFER STOCK (Async)
        public async Task<bool> TransferStockAsync(StockTransferDto dto)
        {
            // A. Find Source Stock
            var sourceStock = await _unitOfWork.WarehouseStocks.GetAllQueryable()
                .FirstOrDefaultAsync(ws => ws.WarehouseId == dto.FromWarehouseId
                                        && ws.ProductPackageId == dto.ProductPackageId);

            if (sourceStock == null || sourceStock.Quantity < dto.Quantity)
            {
                return false;
            }

            // B. Find Destination Stock
            var destStock = await _unitOfWork.WarehouseStocks.GetAllQueryable()
                .FirstOrDefaultAsync(ws => ws.WarehouseId == dto.ToWarehouseId
                                        && ws.ProductPackageId == dto.ProductPackageId);

            // C. Execute Logic
            sourceStock.Quantity -= dto.Quantity;
            _unitOfWork.WarehouseStocks.Update(sourceStock);

            if (destStock != null)
            {
                destStock.Quantity += dto.Quantity;
                _unitOfWork.WarehouseStocks.Update(destStock);
            }
            else
            {
                var newStock = new WarehouseStock
                {
                    WarehouseId = dto.ToWarehouseId,
                    ProductPackageId = dto.ProductPackageId,
                    Quantity = dto.Quantity,
                    MinStockLevel = 0
                };
                await _unitOfWork.WarehouseStocks.CreateAsync(newStock);
            }

            // D. Log the Transfer
            var transferLog = new StockTransferLog
            {
                TransferDate = DateTime.UtcNow,
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                ProductPackageId = dto.ProductPackageId,
                Quantity = dto.Quantity
            };

            await _unitOfWork.StockTransferLogs.CreateAsync(transferLog);
            await _unitOfWork.SaveChangesAsync(); 

            return true;
        }

        // 4. GET WAREHOUSE STOCK (Async)
        public async Task<IEnumerable<WarehouseStockDto>> GetWarehouseStockAsync(int warehouseId)
        {
            var stockItems = _unitOfWork.WarehouseStocks.GetAllQueryable()
                .Where(w => w.WarehouseId == warehouseId);

            var query = from stock in stockItems
                        join pkg in _unitOfWork.ProductPackages.GetAllQueryable() on stock.ProductPackageId equals pkg.Id
                        join pt in _unitOfWork.PackageTypes.GetAllQueryable() on pkg.PackageTypeId equals pt.Id
                        join var in _unitOfWork.ProductVariations.GetAllQueryable() on pkg.ProductVariationId equals var.Id
                        join prod in _unitOfWork.Products.GetAllQueryable() on var.ProductId equals prod.Id
                        select new WarehouseStockDto
                        {
                            StockId = stock.Id,
                            ProductPackageId = pkg.Id,
                            ProductName = prod.Name,
                            VariationName = var.Name,
                            PackageName = pt.Name,
                            Quantity = stock.Quantity
                        };

            return await query.ToListAsync();
        }

        // 5. GET DETAILS (Async)
        public async Task<WarehouseDetailsDto?> GetWarehouseDetailsAsync(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.FindByIdAsync(id);

            if (warehouse == null) return null;

            // Reuse the async method above
            var stockList = await GetWarehouseStockAsync(id);

            return new WarehouseDetailsDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                IsMainWarehouse = warehouse.IsMainWarehouse,
                StockItems = stockList
            };
        }

        // 6. GET TRANSFER LOGS (Async)
        public async Task<IEnumerable<StockTransferLogDto>> GetTransferLogsAsync()
        {
            var logs = _unitOfWork.StockTransferLogs.GetAllQueryable();
            var warehouses = _unitOfWork.Warehouses.GetAllQueryable();
            var packages = _unitOfWork.ProductPackages.GetAllQueryable();
            var variations = _unitOfWork.ProductVariations.GetAllQueryable();
            var products = _unitOfWork.Products.GetAllQueryable();
            var packageTypes = _unitOfWork.PackageTypes.GetAllQueryable();

            var query = from log in logs
                        join whFrom in warehouses on log.FromWarehouseId equals whFrom.Id
                        join whTo in warehouses on log.ToWarehouseId equals whTo.Id
                        join pkg in packages on log.ProductPackageId equals pkg.Id
                        join pt in packageTypes on pkg.PackageTypeId equals pt.Id
                        join var in variations on pkg.ProductVariationId equals var.Id
                        join prod in products on var.ProductId equals prod.Id
                        orderby log.TransferDate descending
                        select new StockTransferLogDto
                        {
                            Id = log.Id,
                            Date = log.TransferDate,
                            FromWarehouse = whFrom.Name,
                            ToWarehouse = whTo.Name,
                            ProductName = prod.Name,
                            VariationName = var.Name,
                            PackageType = pt.Name,
                            Quantity = log.Quantity
                        };

            return await query.ToListAsync();
        }
    }
}
