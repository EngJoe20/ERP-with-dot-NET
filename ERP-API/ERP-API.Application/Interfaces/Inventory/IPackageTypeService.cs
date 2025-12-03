using ERP_API.Application.DTOs.Inventory.Packages;
using ERP_API.DataAccess.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Inventory
{
    public interface IPackageTypeService
    {
        Task<IEnumerable<PackageTypeItemDto>> GetAllPackageTypesAsync();
        Task<PackageType> AddPackageTypeAsync(PackageTypeInsertDto dto);
        Task<PackageDetailsDto?> GetPackageDetailsWithProductsAsync(int id);
    }
}
