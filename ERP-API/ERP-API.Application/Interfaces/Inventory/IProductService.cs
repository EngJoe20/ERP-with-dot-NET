using ERP_API.Application.DTOs.Inventory.Product;
using ERP_API.Application.DTOs.Inventory.Product.Responses;
using ERP_API.DataAccess.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> AddProductAsync(ProductInsertDto dto);

        Task<IEnumerable<ProductSummaryDto>> GetAllProductsAsync();

        Task<ProductResponseDto?> GetProductByIdAsync(int id);

        Task<VariationResponseDto> AddVariationAsync(int productId, VariationInsertDto dto);

        Task<PackageResponseDto> AddPackageAsync(int variationId, PackageLinkInsertDto dto);
    }
}
