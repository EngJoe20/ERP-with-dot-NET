using ERP_API.Application.DTOs.Purchasing.PurchaseReturn;
using ERP_API.Application.Interfaces.Purchasing;
using ERP_API.DataAccess.Entities.Purchasing;
using ERP_API.DataAccess.Entities.Warehouse;
using ERP_API.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP_API.Application.Services.Purchasing
{
    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public PurchaseReturnService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseReturnResponseDto> CreateReturnAsync(CreatePurchaseReturnDto dto, Guid userId)
        {
            // Validate supplier exists
            var supplier = await _unitOfWork.Suppliers.FindByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new Exception("Supplier not found");

            // Calculate totals
            decimal totalAmount = 0;
            var returnItems = new List<PurchaseReturnItem>();

            foreach (var itemDto in dto.Items)
            {
                var productPackage = await _unitOfWork.ProductPackages.FindByIdAsync(itemDto.ProductPackageId);
                if (productPackage == null)
                    throw new Exception($"Product package {itemDto.ProductPackageId} not found");

                var itemTotal = itemDto.Quantity * itemDto.UnitCount * itemDto.UnitPrice;
                totalAmount += itemTotal;

                returnItems.Add(new PurchaseReturnItem
                {
                    ProductPackageId = itemDto.ProductPackageId,
                    Quantity = itemDto.Quantity,
                    Price = itemDto.UnitPrice,
                    Total = itemTotal
                });

                // Decrease inventory (return to supplier = decrease our stock)
                var totalQuantityToDeduct = itemDto.Quantity * itemDto.UnitCount;
                UpdateInventoryAsync(itemDto.ProductPackageId, -totalQuantityToDeduct);
            }

            // Create return
            var purchaseReturn = new PurchaseReturn
            {
                SupplierId = dto.SupplierId,
                UserId = userId,
                ReturnDate = dto.ReturnDate,
                TotalAmount = totalAmount,
                Reason = dto.Reason,
                Items = returnItems
            };

            await _unitOfWork.PurchaseReturns.CreateAsync(purchaseReturn);
            await _unitOfWork.SaveChangesAsync();

            return await GetReturnByIdAsync(purchaseReturn.Id)
                ?? throw new Exception("Failed to retrieve created return");
        }

        public async Task<PurchaseReturnResponseDto?> GetReturnByIdAsync(int id)
        {
            var returnEntity = _unitOfWork.PurchaseReturns
                .GetAllQueryable()
                .Include(r => r.Supplier)
                .Include(r => r.Items)
                    .ThenInclude(item => item.ProductPackage)
                        .ThenInclude(pp => pp.ProductVariation)
                            .ThenInclude(pv => pv.Product)
                .FirstOrDefault(r => r.Id == id);

            if (returnEntity == null) return null;

            return new PurchaseReturnResponseDto
            {
                Id = returnEntity.Id,
                ReturnDate = returnEntity.ReturnDate,
                SupplierName = returnEntity.Supplier.Name,
                SupplierId = returnEntity.SupplierId,
                TotalAmount = returnEntity.TotalAmount,
                Reason = returnEntity.Reason,
                Items = returnEntity.Items.Select(item => new PurchaseReturnItemResponseDto
                {
                    Id = item.Id,
                    ProductCode = item.ProductPackage.Barcode,
                    ProductName = item.ProductPackage.ProductVariation.Product.Name,
                    Quantity = item.Quantity,
                    UnitCount = 1, // Adjust based on your logic
                    UnitPrice = item.Price,
                    Total = item.Total
                }).ToList()
            };
        }

        public async Task<List<PurchaseReturnListItemDto>> GetAllReturnsAsync()
        {
            var returns = _unitOfWork.PurchaseReturns
                .GetAllQueryable()
                .Include(r => r.Supplier)
                .OrderByDescending(r => r.ReturnDate)
                .ToList();

            return returns.Select((r, index) => new PurchaseReturnListItemDto
            {
                RowNumber = index + 1,
                SupplierName = r.Supplier.Name,
                ReturnDate = r.ReturnDate,
                TotalAmount = r.TotalAmount
            }).ToList();
        }

        public async Task<List<PurchaseReturnListItemDto>> GetReturnsBySupplierAsync(Guid supplierId)
        {
            var returns = _unitOfWork.PurchaseReturns
                .GetAllQueryable()
                .Where(r => r.SupplierId == supplierId)
                .Include(r => r.Supplier)
                .OrderByDescending(r => r.ReturnDate)
                .ToList();

            return returns.Select((r, index) => new PurchaseReturnListItemDto
            {
                RowNumber = index + 1,
                SupplierName = r.Supplier.Name,
                ReturnDate = r.ReturnDate,
                TotalAmount = r.TotalAmount
            }).ToList();
        }

        public async Task<bool> DeleteReturnAsync(int id)
        {
            var returnEntity = _unitOfWork.PurchaseReturns
                .GetAllQueryable()
                .Include(r => r.Items)
                .FirstOrDefault(r => r.Id == id);

            if (returnEntity == null) return false;

            // Reverse inventory changes
            foreach (var item in returnEntity.Items)
            {
                UpdateInventoryAsync(item.ProductPackageId, item.Quantity);
            }

            await _unitOfWork.PurchaseReturns.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async void UpdateInventoryAsync(int productPackageId, decimal quantityChange)
        {
            var mainWarehouse = _unitOfWork.Warehouses
                .GetAllQueryable()
                .FirstOrDefault(w => w.IsMainWarehouse);

            if (mainWarehouse == null)
                throw new Exception("Main warehouse not found");

            var stock = _unitOfWork.WarehouseStocks
                .GetAllQueryable()
                .FirstOrDefault(s => s.WarehouseId == mainWarehouse.Id && s.ProductPackageId == productPackageId);

            if (stock == null && quantityChange > 0)
            {
                stock = new WarehouseStock
                {
                    WarehouseId = mainWarehouse.Id,
                    ProductPackageId = productPackageId,
                    Quantity = quantityChange,
                    MinStockLevel = 0
                };
                await _unitOfWork.WarehouseStocks.CreateAsync(stock);
            }
            else if (stock != null)
            {
                stock.Quantity += quantityChange;
                if (stock.Quantity < 0)
                    throw new Exception("Cannot have negative stock");
                _unitOfWork.WarehouseStocks.Update(stock);
            }
        }
    }
}