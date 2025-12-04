using ERP_API.Application.DTOs.Purchasing.PurchaseInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Purchasing
{
    public interface IPurchaseInvoiceService
    {
        Task<PurchaseInvoiceResponseDto> CreateInvoiceAsync(CreatePurchaseInvoiceDto dto, Guid userId);
        Task<PurchaseInvoiceResponseDto?> GetInvoiceByIdAsync(int id);
        Task<List<PurchaseInvoiceListItemDto>> GetAllInvoicesAsync();
        Task<List<PurchaseInvoiceListItemDto>> GetInvoicesBySupplierAsync(Guid supplierId);
        Task<bool> DeleteInvoiceAsync(int id);
    }
}
