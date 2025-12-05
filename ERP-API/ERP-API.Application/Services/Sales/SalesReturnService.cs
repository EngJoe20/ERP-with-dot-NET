using ERP_API.Application.DTOs.Sales.SalesReturn;
using ERP_API.Application.Interfaces.Sales;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Sales;
using ERP_API.DataAccess.Entities.Warehouse;
using ERP_API.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP_API.Application.Services.Sales
{
    public class SalesReturnService : ISalesReturnService
    {
        private readonly IErpUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesReturnService(IErpUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SalesReturnResponseDto> CreateReturnAsync(CreateSalesReturnDto dto)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User not authenticated");

            //conv ti int
            var appUser = await _unitOfWork.UserManager.FindByIdAsync(userIdString);
            if (appUser == null)
                throw new UnauthorizedAccessException("User not found");

            //int userId = appUser.myId;

            // Validate customer exists
            var customer = await _unitOfWork.Customers.FindByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new Exception("Customer not found");

            // Calculate totals
            decimal totalAmount = 0;
            var returnItems = new List<SalesReturnItem>();

            foreach (var itemDto in dto.Items)
            {
                var productPackage = await _unitOfWork.ProductPackages.FindByIdAsync(itemDto.ProductPackageId);
                if (productPackage == null)
                    throw new Exception($"Product package {itemDto.ProductPackageId} not found");

                var itemTotal = itemDto.Quantity * itemDto.UnitCount * itemDto.Price;
                totalAmount += itemTotal;

                returnItems.Add(new SalesReturnItem
                {
                    ProductPackageId = itemDto.ProductPackageId,
                    Quantity = itemDto.Quantity,
                    Price = itemDto.Price,
                    Total = itemTotal
                });

                // Increase inventory (customer returned = increase our stock)
                var totalQuantityToAdd = itemDto.Quantity * itemDto.UnitCount;
                UpdateInventory(itemDto.ProductPackageId, totalQuantityToAdd);
            }

            // Create return
            var salesReturn = new SalesReturn
            {
                CustomerId = dto.CustomerId,
                ReturnDate = dto.ReturnDate,
                TotalAmount = totalAmount,
                Reason = dto.Reason,
                //UserId = userId,
                Items = returnItems
            };

            await _unitOfWork.SalesReturns.CreateAsync(salesReturn);


            var customerTransaction = new CustomerTransaction
            {
                CustomerId = dto.CustomerId,
                CustomerTransactionType = CustomerTransactionType.SalesReturn,
                TransactionDate = dto.ReturnDate,
                Amount = salesReturn.TotalAmount,
                Direction = CustomerTransactionDirection.In, //العميل رجع بضاعة = خصم من ديونه (in => استلمنا)
                Description = $"Sales Return #{salesReturn.Id}"
            };

            await _unitOfWork.CustomerTransactions.CreateAsync(customerTransaction);

            await _unitOfWork.SaveChangesAsync();

            return await GetReturnByIdAsync(salesReturn.Id)
                ?? throw new Exception("Failed to retrieve created return");
        }

        public async Task<SalesReturnResponseDto?> GetReturnByIdAsync(int id)
        {
            var returnEntity = _unitOfWork.SalesReturns
                .GetAllQueryable()
                .Include(r => r.Customer)
                .Include(r => r.Items)
                    .ThenInclude(item => item.ProductPackage)
                        .ThenInclude(pp => pp.ProductVariation)
                            .ThenInclude(pv => pv.Product)
                .FirstOrDefault(r => r.Id == id);

            if (returnEntity == null) return null;

            return new SalesReturnResponseDto
            {
                Id = returnEntity.Id,
                ReturnDate = returnEntity.ReturnDate,
                CustomerName = returnEntity.Customer.CustomerName,
                CustomerId = returnEntity.CustomerId,
                TotalAmount = returnEntity.TotalAmount,
                Reason = returnEntity.Reason,
                Items = returnEntity.Items.Select(item => new SalesReturnItemResponseDto
                {
                    Id = item.Id,
                    ProductCode = item.ProductPackage.Barcode,
                    ProductName = item.ProductPackage.ProductVariation.Product.Name,
                    Quantity = item.Quantity,
                    UnitCount = 1, 
                    Price = item.Price,
                    Total = item.Total
                }).ToList()
            };
        }

        public async Task<List<SalesReturnListItemDto>> GetAllReturnsAsync()
        {
            var returns = _unitOfWork.SalesReturns
                .GetAllQueryable()
                .Include(r => r.Customer)
                .OrderByDescending(r => r.ReturnDate)
                .ToList();

            return returns.Select((r, index) => new SalesReturnListItemDto
            {
                RowNumber = index + 1,
                CustomerName = r.Customer.CustomerName,
                ReturnDate = r.ReturnDate,
                TotalAmount = r.TotalAmount
            }).ToList();
        }

        public async Task<List<SalesReturnListItemDto>> GetReturnsByCustomerAsync(int customerId)
        {
            var returns = _unitOfWork.SalesReturns
                .GetAllQueryable()
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.ReturnDate)
                .ToList();

            return returns.Select((r, index) => new SalesReturnListItemDto
            {
                RowNumber = index + 1,
                CustomerName = r.Customer.CustomerName,
                ReturnDate = r.ReturnDate,
                TotalAmount = r.TotalAmount
            }).ToList();
        }

        public async Task<bool> DeleteReturnAsync(int id)
        {
            var returnEntity = _unitOfWork.SalesReturns
                .GetAllQueryable()
                .Include(r => r.Items)
                .FirstOrDefault(r => r.Id == id);

            if (returnEntity == null) return false;

            // Reverse inventory changes
            foreach (var item in returnEntity.Items)
            {
                UpdateInventory(item.ProductPackageId, -item.Quantity);
            }

            await _unitOfWork.SalesReturns.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void UpdateInventory(int productPackageId, decimal quantityChange)
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
                _unitOfWork.WarehouseStocks.CreateAsync(stock);
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