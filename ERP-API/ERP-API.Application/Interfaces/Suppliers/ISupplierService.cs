using ERP_API.Application.DTOs.Suppliers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Suppliers
{
    public interface ISupplierService
    {
        // Read operations
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto?> GetSupplierAsync(int id);
        Task<SupplierDetailsDto?> GetSupplierDetailsAsync(int id);
        Task<IEnumerable<SupplierTransactionDto>> GetSupplierTransactionsAsync(int supplierId);

        // Write operations
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto);
        Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierDto dto);
        Task<bool> DeleteSupplierAsync(int id);

        // Helper methods
        Task RecalculateSupplierBalanceAsync(int supplierId);
    }
}
