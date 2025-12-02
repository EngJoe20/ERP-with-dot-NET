using ERP_API.Application.Interfaces;
using ERP_API.Application.DTOs.Inventory.Packages;
using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.Application.Interfaces.Inventory;

namespace ERP_API.Application.Services
{
    public class PackageTypeService : IPackageTypeService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public PackageTypeService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. GET ALL (Async)
        public async Task<IEnumerable<PackageTypeItemDto>> GetAllPackageTypesAsync()
        {
            // Use Queryable + Select + ToListAsync for maximum efficiency
            return await _unitOfWork.PackageTypes.GetAllQueryable()
                .Select(pt => new PackageTypeItemDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    UnitOfMeasurement = pt.UnitOfMeasurement
                })
                .ToListAsync(); // ✅ Executed Asynchronously
        }

        // 2. ADD (Async)
        public async Task<PackageType> AddPackageTypeAsync(PackageTypeInsertDto dto)
        {
            var entity = new PackageType
            {
                Name = dto.Name,
                Description = dto.Description,
                UnitOfMeasurement = dto.UnitOfMeasurement
            };

            await _unitOfWork.PackageTypes.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync(); // ✅ Saves to SQL Asynchronously

            return entity;
        }

        // 3. GET DETAILS (Async)
        public async Task<PackageDetailsDto?> GetPackageDetailsWithProductsAsync(int packageTypeId)
        {
            // 1. Get the Package Info (Async)
            var packageType = await _unitOfWork.PackageTypes.FindByIdAsync(packageTypeId);

            if (packageType == null) return null;

            // 2. Get Products (Async Query)
            var productNames = await _unitOfWork.ProductPackages.GetAllQueryable()
                .Where(pkg => pkg.PackageTypeId == packageTypeId)
                .Join(_unitOfWork.ProductVariations.GetAllQueryable(),
                      pkg => pkg.ProductVariationId,
                      var => var.Id,
                      (pkg, var) => var)
                .Join(_unitOfWork.Products.GetAllQueryable(),
                      var => var.ProductId,
                      prod => prod.Id,
                      (var, prod) => prod.Name + " - " + var.Name)
                .ToListAsync(); // ✅ Execute Query Asynchronously

            return new PackageDetailsDto
            {
                PackageTypeName = packageType.Name,
                UnitOfMeasurement = packageType.UnitOfMeasurement,
                ProductsUsingThis = productNames
            };
        }
    }
}
