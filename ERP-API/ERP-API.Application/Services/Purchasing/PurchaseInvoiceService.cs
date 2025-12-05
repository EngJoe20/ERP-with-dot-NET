using ERP_API.Application.DTOs.Purchasing.PurchaseInvoice;
using ERP_API.Application.Interfaces.Purchasing;
using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.Purchasing;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Entities.Warehouse;
using ERP_API.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP_API.Application.Services.Purchasing
{
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly IErpUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PurchaseInvoiceService(
        IErpUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PurchaseInvoiceResponseDto> CreateInvoiceAsync(CreatePurchaseInvoiceDto dto)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User not authenticated");

            //conv ti int
            var appUser = await _unitOfWork.UserManager.FindByIdAsync(userIdString);
            if (appUser == null)
                throw new UnauthorizedAccessException("User not found");

            //int userId = appUser.myId;

            // Validate supplier exists
            var supplier = await _unitOfWork.Suppliers.FindByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new Exception("Supplier not found");

            // Calculate totals
            decimal totalAmount = 0;
            var invoiceItems = new List<PurchaseInvoiceItem>();

            foreach (var itemDto in dto.Items)
            {
                var productPackage = await _unitOfWork.ProductPackages.FindByIdAsync(itemDto.ProductPackageId);
                if (productPackage == null)
                    throw new Exception($"Product package {itemDto.ProductPackageId} not found");

                var itemTotal = itemDto.Quantity * itemDto.UnitPrice;
                totalAmount += itemTotal;

                invoiceItems.Add(new PurchaseInvoiceItem
                {
                    ProductPackageId = itemDto.ProductPackageId,
                    Quantity = itemDto.Quantity,
                    PurchasePrice = itemDto.UnitPrice,
                    Total = itemTotal
                });

                // Update inventory - increase stock
                UpdateInventoryAsync(itemDto.ProductPackageId, itemDto.Quantity, itemDto.UnitPrice);

                // Update average purchase price
                UpdateAveragePurchasePriceAsync(productPackage, itemDto.Quantity, itemDto.UnitPrice);
            }

            // Calculate net amount after discount
            var netAmount = totalAmount - (dto.Discount ?? 0);

            // Get supplier balance
            var balanceBefore = supplier.TotalBalance;
            var balanceAfter = balanceBefore + netAmount - (dto.PaymentOrderAmount ?? 0);

            // Generate invoice number
            var invoiceNumber = GenerateInvoiceNumber();

            // Create invoice
            var invoice = new PurchaseInvoice
            {
                InvoiceNumber = invoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                SupplierId = dto.SupplierId,
                //UserId = userId,
                TotalAmount = totalAmount,
                NetAmount = netAmount,
                Discount = dto.Discount,
                PaymentOrderAmount = dto.PaymentOrderAmount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                Items = invoiceItems
            };

            await _unitOfWork.PurchaseInvoices.CreateAsync(invoice);

            //txn supp
            var supplierTransaction = new SupplierTransaction
            {
                SupplierId = dto.SupplierId,
                SupplierTransactionType = SupplierTransactionType.PurchaseInvoice,
                TransactionDate = dto.InvoiceDate,
                Amount = invoice.NetAmount,
                Direction = SupplierTransactionDirection.In, //احنا مديونين للمورد (in => دين علينا)
                Description = $"Purchase Invoice {invoice.InvoiceNumber}"
            };

            await _unitOfWork.SupplierTransactions.CreateAsync(supplierTransaction);

            //Update supplier balance
            supplier.TotalBalance = balanceAfter;
            _unitOfWork.Suppliers.Update(supplier);
 
            await _unitOfWork.SaveChangesAsync();
            return await GetInvoiceByIdAsync(invoice.Id) ?? throw new Exception("Failed to retrieve created invoice");
            
        }

        public async Task<PurchaseInvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = _unitOfWork.PurchaseInvoices
                .GetAllQueryable()
                .Include(i => i.Supplier)
                .Include(i => i.Items)
                    .ThenInclude(item => item.ProductPackage)
                        .ThenInclude(pp => pp.ProductVariation)
                            .ThenInclude(pv => pv.Product)
                .Include(i => i.Items)
                    .ThenInclude(item => item.ProductPackage)
                        .ThenInclude(pp => pp.PackageType)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null) return null;

       
            return new PurchaseInvoiceResponseDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                SupplierName = invoice.Supplier.SupplierName,
                SupplierId = invoice.SupplierId,
                TotalAmount = invoice.TotalAmount,
                NetAmount = invoice.NetAmount,
                Discount = invoice.Discount ?? 0, 
                PaymentOrderAmount = invoice.PaymentOrderAmount,
                BalanceBefore = invoice.BalanceBefore,
                BalanceAfter = invoice.BalanceAfter,
                CreatedDate = invoice.CreatedDate,
                Items = invoice.Items.Select(item => new PurchaseInvoiceItemResponseDto
                {
                    Id = item.Id,
                    ProductName = item.ProductPackage.ProductVariation.Product.Name,
                    PackageTypeName = item.ProductPackage.PackageType.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.PurchasePrice,
                    Total = item.Total
                }).ToList()
            };
        }

        public async Task<List<PurchaseInvoiceListItemDto>> GetAllInvoicesAsync()
        {
            return _unitOfWork.PurchaseInvoices
                .GetAllQueryable()
                .Include(i => i.Supplier)
                .OrderByDescending(i => i.CreatedDate)
                .Select(i => new PurchaseInvoiceListItemDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    SupplierName = i.Supplier.SupplierName,
                    TotalAmount = i.TotalAmount,
                    NetAmount = i.NetAmount
                })
                .ToList();
        }

        public async Task<List<PurchaseInvoiceListItemDto>> GetInvoicesBySupplierAsync(int supplierId)
        {
            return _unitOfWork.PurchaseInvoices
                .GetAllQueryable()
                .Where(i => i.SupplierId == supplierId)
                .Include(i => i.Supplier)
                .OrderByDescending(i => i.CreatedDate)
                .Select(i => new PurchaseInvoiceListItemDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    SupplierName = i.Supplier.SupplierName,
                    TotalAmount = i.TotalAmount,
                    NetAmount = i.NetAmount
                })
                .ToList();
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = _unitOfWork.PurchaseInvoices
                .GetAllQueryable()
                .Include(i => i.Items)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null) return false;

            // Reverse inventory changes
            foreach (var item in invoice.Items)
            {
                UpdateInventoryAsync(item.ProductPackageId, -item.Quantity, item.PurchasePrice);
            }

            await _unitOfWork.PurchaseInvoices.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Helper methods
        private async void UpdateInventoryAsync(int productPackageId, int quantity, decimal purchasePrice)
        {
            // Find main warehouse
            var mainWarehouse = _unitOfWork.Warehouses
                .GetAllQueryable()
                .FirstOrDefault(w => w.IsMainWarehouse);

            if (mainWarehouse == null)
                throw new Exception("Main warehouse not found");

            var stock = _unitOfWork.WarehouseStocks
                .GetAllQueryable()
                .FirstOrDefault(s => s.WarehouseId == mainWarehouse.Id && s.ProductPackageId == productPackageId);

            if (stock == null)
            {
                // Create new stock entry
                stock = new WarehouseStock
                {
                    WarehouseId = mainWarehouse.Id,
                    ProductPackageId = productPackageId,
                    Quantity = quantity,
                    MinStockLevel = 0
                };
                await _unitOfWork.WarehouseStocks.CreateAsync(stock);
            }
            else
            {
                stock.Quantity += quantity;
                _unitOfWork.WarehouseStocks.Update(stock);
            }
        }

        private async void UpdateAveragePurchasePriceAsync(ProductPackage productPackage, int quantity, decimal purchasePrice)
        {
            var currentStock = _unitOfWork.WarehouseStocks
                .GetAllQueryable()
                .Where(s => s.ProductPackageId == productPackage.Id)
                .Sum(s => s.Quantity);

            if (currentStock > 0)
            {
                var totalValue = (productPackage.PurchasePrice * currentStock) + (purchasePrice * quantity);
                var totalQuantity = currentStock + quantity;
                productPackage.PurchasePrice = totalValue / totalQuantity;
            }
            else
            {
                productPackage.PurchasePrice = purchasePrice;
            }

            _unitOfWork.ProductPackages.Update(productPackage);
        }

        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _unitOfWork.PurchaseInvoices
                .GetAllQueryable()
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            var nextNumber = (lastInvoice?.Id ?? 0) + 1;
            return $"PI-{DateTime.Now:yyyyMMdd}-{nextNumber:D5}";
        }
    }
}