using ERP_API.Application.Interfaces;
using ERP_API.Application.DTOs.Inventory.Product;
using ERP_API.Application.DTOs.Inventory.Product.Responses;
using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.Entities.Inventory;
using ERP_API.DataAccess.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP_API.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public ProductService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==========================================================
        // 1. ADD PRODUCT (Async)
        // ==========================================================
        public async Task<ProductResponseDto> AddProductAsync(ProductInsertDto dto)
        {
            // 1. Create Product Entity
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };

            await productRepo.CreateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // 2. Generate Smart SKU
            string smartSku = await GenerateSmartSKUAsync(product.Id);

            // 3. Create Variation Entity
            var variation = new ProductVariation
            {
                ProductId = product.Id,
                Name = dto.VariationName,
                Flavor = dto.Flavor,
                Size = dto.Size,
                SKU = smartSku
            };

            await variationRepo.CreateAsync(variation);
            await _unitOfWork.SaveChangesAsync();

            // 4. Create Package Entity
            var package = new ProductPackage
            {
                ProductVariationId = variation.Id,
                PackageTypeId = dto.PackageTypeId,
                QinP = dto.QinP,
                PurchasePrice = dto.PurchasePrice,
                SalesPrice = dto.SalesPrice,
                Barcode = GenerateBarcode(variation.Id)
            };

            await packageRepo.CreateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            // 5. Add Initial Stock (Async)
            await AddInitialStockToMainAsync(package.Id, dto.InitialQuantity);

            // 6. Fetch Names for Response (Async)
            var packageTypeEntity = await _unitOfWork.PackageTypes.FindByIdAsync(dto.PackageTypeId);
            string pkgName = packageTypeEntity != null ? packageTypeEntity.Name : "Unknown";

            // Return DTO
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = "General",
                Variations = new List<VariationResponseDto>
                {
                    new VariationResponseDto
                    {
                        Id = variation.Id,
                        Name = variation.Name,
                        Flavor = variation.Flavor,
                        SKU = variation.SKU,
                        Packages = new List<PackageResponseDto>
                        {
                            new PackageResponseDto
                            {
                                Id = package.Id,
                                PackageTypeName = pkgName,
                                QinP = package.QinP,
                                SalesPrice = package.SalesPrice,
                                Barcode = package.Barcode
                            }
                        }
                    }
                }
            };
        }

        // ==========================================================
        // 2. ADD VARIATION (Async)
        // ==========================================================
        public async Task<VariationResponseDto> AddVariationAsync(int productId, VariationInsertDto dto)
        {
            string smartSku = await GenerateSmartSKUAsync(productId);

            var variation = new ProductVariation
            {
                ProductId = productId,
                Name = dto.VariationName,
                Flavor = dto.Flavor,
                Size = dto.Size,
                SKU = smartSku
            };

            await variationRepo.CreateAsync(variation);
            await _unitOfWork.SaveChangesAsync();

            var package = new ProductPackage
            {
                ProductVariationId = variation.Id,
                PackageTypeId = dto.PackageTypeId,
                QinP = dto.QinP,
                PurchasePrice = dto.PurchasePrice,
                SalesPrice = dto.SalesPrice,
                Barcode = GenerateBarcode(variation.Id)
            };

            await packageRepo.CreateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            await AddInitialStockToMainAsync(package.Id, dto.InitialQuantity);

            var packageTypeEntity = await _unitOfWork.PackageTypes.FindByIdAsync(dto.PackageTypeId);
            string pkgName = packageTypeEntity != null ? packageTypeEntity.Name : "Unknown";

            return new VariationResponseDto
            {
                Id = variation.Id,
                Name = variation.Name,
                Flavor = variation.Flavor,
                SKU = variation.SKU,
                Packages = new List<PackageResponseDto>
                {
                    new PackageResponseDto
                    {
                        Id = package.Id,
                        PackageTypeName = pkgName,
                        QinP = package.QinP,
                        SalesPrice = package.SalesPrice,
                        Barcode = package.Barcode
                    }
                }
            };
        }

        // ==========================================================
        // 3. ADD PACKAGE (Async)
        // ==========================================================
        public async Task<PackageResponseDto> AddPackageAsync(int variationId, PackageLinkInsertDto dto)
        {
            var package = new ProductPackage
            {
                ProductVariationId = variationId,
                PackageTypeId = dto.PackageTypeId,
                QinP = dto.QinP,
                PurchasePrice = dto.PurchasePrice,
                SalesPrice = dto.SalesPrice,
                Barcode = GenerateBarcode(variationId)
            };

            await packageRepo.CreateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            await AddInitialStockToMainAsync(package.Id, dto.InitialQuantity);

            var packageTypeEntity = await _unitOfWork.PackageTypes.FindByIdAsync(dto.PackageTypeId);
            string pkgName = packageTypeEntity != null ? packageTypeEntity.Name : "Unknown";

            return new PackageResponseDto
            {
                Id = package.Id,
                PackageTypeName = pkgName,
                QinP = package.QinP,
                SalesPrice = package.SalesPrice,
                Barcode = package.Barcode
            };
        }

        // ==========================================================
        // 4. GET ALL PRODUCTS (Async)
        // ==========================================================
        public async Task<IEnumerable<ProductSummaryDto>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetAllQueryable()
                .Select(p => new ProductSummaryDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    VariationCount = p.Variations.Count()
                })
                .ToListAsync(); // ✅ Async Execution
        }

        // ==========================================================
        // 5. GET PRODUCT BY ID (Async)
        // ==========================================================
        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var query = _unitOfWork.Products.GetAllQueryable()
                .Where(p => p.Id == id)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryName = p.Category != null ? p.Category.Name : "Uncategorized",

                    Variations = p.Variations.Select(v => new VariationResponseDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Flavor = v.Flavor,
                        SKU = v.SKU,

                        Packages = v.ProductPackages.Select(pkg => new PackageResponseDto
                        {
                            Id = pkg.Id,
                            PackageTypeName = pkg.PackageType.Name,
                            Barcode = pkg.Barcode,

                            QinP = pkg.QinP,
                            SalesPrice = pkg.SalesPrice,

                            
                            PurchasePrice = pkg.PurchasePrice,

                           
                            CurrentStock = pkg.WarehouseStocks.Sum(ws => ws.Quantity)
                        }).ToList()
                    }).ToList()
                });

            return await query.FirstOrDefaultAsync();
        }

        // ==========================================================
        // 🛠️ HELPERS (Async where needed)
        // ==========================================================

        private async Task AddInitialStockToMainAsync(int packageId, decimal quantity)
        {
            if (quantity > 0)
            {
                // Find Main Warehouse (Async)
                var mainWarehouse = await warehouseRepo.GetAllQueryable()
                    .FirstOrDefaultAsync(w => w.IsMainWarehouse);

                // Safety: Auto-create
                if (mainWarehouse == null)
                {
                    mainWarehouse = new Warehouse
                    {
                        Name = "Main Virtual Warehouse",
                        Location = "System (Virtual)",
                        IsMainWarehouse = true
                    };
                    await warehouseRepo.CreateAsync(mainWarehouse);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Add Stock
                var stock = new WarehouseStock
                {
                    WarehouseId = mainWarehouse.Id,
                    ProductPackageId = packageId,
                    Quantity = quantity,
                    MinStockLevel = 0
                };
                await stockRepo.CreateAsync(stock);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateSmartSKUAsync(int productId)
        {
            // Use CountAsync for database efficiency
            var count = await variationRepo.GetAllQueryable()
                .Where(v => v.ProductId == productId)
                .CountAsync();

            var nextNumber = count + 1;
            return $"PROD{productId}-VAR{nextNumber.ToString("D3")}";
        }

        private string GenerateBarcode(int variationId)
        {
            return $"800{variationId.ToString("D5")}";
        }

        // --- Repository Accessors ---
        private IBaseRepository<Product, int> productRepo => _unitOfWork.Products;
        private IBaseRepository<ProductVariation, int> variationRepo => _unitOfWork.ProductVariations;
        private IBaseRepository<ProductPackage, int> packageRepo => _unitOfWork.ProductPackages;
        private IBaseRepository<Warehouse, int> warehouseRepo => _unitOfWork.Warehouses;
        private IBaseRepository<WarehouseStock, int> stockRepo => _unitOfWork.WarehouseStocks;
    }
}