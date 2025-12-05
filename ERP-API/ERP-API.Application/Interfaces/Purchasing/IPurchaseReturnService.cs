using ERP_API.Application.DTOs.Purchasing.PurchaseReturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Purchasing
{
    public interface IPurchaseReturnService
    {
        Task<PurchaseReturnResponseDto> CreateReturnAsync(CreatePurchaseReturnDto dto);
        Task<PurchaseReturnResponseDto?> GetReturnByIdAsync(int id);
        Task<List<PurchaseReturnListItemDto>> GetAllReturnsAsync();
        Task<List<PurchaseReturnListItemDto>> GetReturnsBySupplierAsync(int supplierId);
        Task<bool> DeleteReturnAsync(int id);
    }
}
