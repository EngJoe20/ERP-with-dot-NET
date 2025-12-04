using ERP_API.Application.DTOs.Sales.SalesInvoice;
using ERP_API.Application.Interfaces.Sales;
using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.Sales;
using ERP_API.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP_API.Application.Services.Sales
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public SalesInvoiceService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SalesInvoiceResponseDto> CreateInvoiceAsync(CreateSalesInvoiceDto dto, Guid userId)
        {
            // Validate customer exists
            var customer = await _unitOfWork.Customers.FindByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new Exception("Customer not found");

            // Validate inventory availability
            foreach (var itemDto in dto.Items)
            {
                var availableStock = GetAvailableStock(itemDto.ProductPackageId);
                var requiredQuantity = itemDto.Quantity * itemDto.UnitCount;

                if (availableStock < requiredQuantity)
                    throw new Exception($"Insufficient stock for product package {itemDto.ProductPackageId}. Available: {availableStock}, Required: {requiredQuantity}");
            }

            // Calculate totals
            decimal totalAmount = 0;
            var invoiceItems = new List<SalesInvoiceItem>();

            foreach (var itemDto in dto.Items)
            {
                var productPackage = await _unitOfWork.ProductPackages.FindByIdAsync(itemDto.ProductPackageId);
                if (productPackage == null)
                    throw new Exception($"Product package {itemDto.ProductPackageId} not found");

                var itemTotal = itemDto.Quantity * itemDto.UnitCount * itemDto.SellingPrice;
                totalAmount += itemTotal;

                invoiceItems.Add(new SalesInvoiceItem
                {
                    ProductPackageId = itemDto.ProductPackageId,
                    Quantity = itemDto.Quantity,
                    UnitCount = itemDto.UnitCount,
                    SellingPrice = itemDto.SellingPrice,
                    Total = itemTotal
                });

                // Update inventory - decrease stock
                var totalQuantityToDeduct = itemDto.Quantity * itemDto.UnitCount;
                UpdateInventory(itemDto.ProductPackageId, -totalQuantityToDeduct);

                // Update average selling price
                UpdateAverageSellingPrice(productPackage, totalQuantityToDeduct, itemDto.SellingPrice);
            }

            // Calculate net amount after discount
            var netAmount = totalAmount - (dto.Discount ?? 0);

            // Get customer balance
            var balanceBefore = customer.InitialBalance ?? 0;
            var balanceAfter = balanceBefore + netAmount - (dto.AmountReceived ?? 0);

            // Generate invoice number
            var invoiceNumber = GenerateInvoiceNumber();

            // Create invoice
            var invoice = new SalesInvoice
            {
                InvoiceNumber = invoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                CustomerId = dto.CustomerId,
                UserId = userId,
                TotalAmount = totalAmount,
                NetAmount = netAmount,
                Discount = dto.Discount,
                AmountReceived = dto.AmountReceived,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                Items = invoiceItems
            };

            await _unitOfWork.SalesInvoices.CreateAsync(invoice);

            // Update customer balance
            customer.InitialBalance = (int)balanceAfter;
            _unitOfWork.Customers.Update(customer);

            await _unitOfWork.SaveChangesAsync();

            return await GetInvoiceByIdAsync(invoice.Id)
                ?? throw new Exception("Failed to retrieve created invoice");
        }

        public async Task<SalesInvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = _unitOfWork.SalesInvoices
                .GetAllQueryable()
                .Include(i => i.Customer)
                .Include(i => i.Items)
                    .ThenInclude(item => item.ProductPackage)
                        .ThenInclude(pp => pp.ProductVariation)
                            .ThenInclude(pv => pv.Product)
                .Include(i => i.Items)
                    .ThenInclude(item => item.ProductPackage)
                        .ThenInclude(pp => pp.PackageType)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null) return null;

            return new SalesInvoiceResponseDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                CustomerName = invoice.Customer.Name,
                CustomerId = invoice.CustomerId,
                TotalAmount = invoice.TotalAmount,
                NetAmount = invoice.NetAmount,
                Discount = invoice.Discount??0,
                AmountReceived = invoice.AmountReceived,
                BalanceBefore = invoice.BalanceBefore,
                BalanceAfter = invoice.BalanceAfter,
                CreatedDate = invoice.CreatedDate,
                Items = invoice.Items.Select(item => new SalesInvoiceItemResponseDto
                {
                    Id = item.Id,
                    ProductName = item.ProductPackage.ProductVariation.Product.Name,
                    PackageTypeName = item.ProductPackage.PackageType.Name,
                    Quantity = item.Quantity,
                    UnitCount = item.UnitCount,
                    SellingPrice = item.SellingPrice,
                    Total = item.Total
                }).ToList()
            };
        }

        public async Task<List<SalesInvoiceListItemDto>> GetAllInvoicesAsync()
        {
            return _unitOfWork.SalesInvoices
                .GetAllQueryable()
                .Include(i => i.Customer)
                .OrderByDescending(i => i.CreatedDate)
                .Select(i => new SalesInvoiceListItemDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    CustomerName = i.Customer.Name,
                    TotalAmount = i.TotalAmount,
                    NetAmount = i.NetAmount
                })
                .ToList();
        }

        public async Task<List<SalesInvoiceListItemDto>> GetInvoicesByCustomerAsync(Guid customerId)
        {
            return _unitOfWork.SalesInvoices
                .GetAllQueryable()
                .Where(i => i.CustomerId == customerId)
                .Include(i => i.Customer)
                .OrderByDescending(i => i.CreatedDate)
                .Select(i => new SalesInvoiceListItemDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    CustomerName = i.Customer.Name,
                    TotalAmount = i.TotalAmount,
                    NetAmount = i.NetAmount
                })
                .ToList();
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = _unitOfWork.SalesInvoices
                .GetAllQueryable()
                .Include(i => i.Items)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null) return false;

            // Reverse inventory changes
            foreach (var item in invoice.Items)
            {
                var totalQuantity = item.Quantity * item.UnitCount;
                UpdateInventory(item.ProductPackageId, totalQuantity);
            }

            await _unitOfWork.SalesInvoices.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Helper methods
        private decimal GetAvailableStock(int productPackageId)
        {
            return _unitOfWork.WarehouseStocks
                .GetAllQueryable()
                .Where(s => s.ProductPackageId == productPackageId)
                .Sum(s => s.Quantity);
        }

        private void UpdateInventory(int productPackageId, decimal quantityChange)
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
                throw new Exception("Stock not found for this product");

            stock.Quantity += quantityChange;

            if (stock.Quantity < 0)
                throw new Exception("Cannot have negative stock");

            _unitOfWork.WarehouseStocks.Update(stock);
        }

        private void UpdateAverageSellingPrice(ProductPackage productPackage, decimal quantity, decimal sellingPrice)
        {
            var currentStock = _unitOfWork.WarehouseStocks
                .GetAllQueryable()
                .Where(s => s.ProductPackageId == productPackage.Id)
                .Sum(s => s.Quantity);

            if (currentStock > 0)
            {
                var totalValue = (productPackage.SalesPrice * currentStock) + (sellingPrice * quantity);
                var totalQuantity = currentStock + quantity;
                productPackage.SalesPrice = totalValue / totalQuantity;
            }
            else
            {
                productPackage.SalesPrice = sellingPrice;
            }

            _unitOfWork.ProductPackages.Update(productPackage);
        }

        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _unitOfWork.SalesInvoices
                .GetAllQueryable()
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            var nextNumber = (lastInvoice?.Id ?? 0) + 1;
            return $"SI-{DateTime.Now:yyyyMMdd}-{nextNumber:D5}";
        }
    }
}